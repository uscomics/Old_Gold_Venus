namespace Player
{
	using UnityEngine;
	using General;
	using System.Collections;

	public class Walk : MonoBehaviour 
	{
		public float walkSpeed = 3.0f;
		public AudioSource footsteps;

		private GameObject player;
		private Player playerScript;
		private Animator animator;
		private Rigidbody playerRigidbody;
		private int ANIMATION_BASE_LAYER;

		void FixedUpdate()
		{
			if (null == player)
			{
				InitData();
			} // if

			bool isWalking = MovementType.WALKING == playerScript.movementInfo.movementType;
			
			if (!isWalking)
			{
				return;
			} // if

			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");

			PlayerWalk(h, v);
		} // FixedUpdate

		void Update()
		{
			if (null == player)
			{
				InitData();
			} // if
			AudioManagement();
		} // Update
		
		void PlayerWalk(float inHorizontal, float inVertical)
		{
			Vector3 targetDirection = new Vector3(inHorizontal, 0.0f, inVertical);
			Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
			Quaternion newRotation = Quaternion.Lerp(playerRigidbody.rotation, targetRotation, playerScript.movementInfo.turnSmoothing * Time.deltaTime);
			
			playerRigidbody.MoveRotation(newRotation);
		} // PlayerWalk

		void AudioManagement()
		{
			if (ANIMATION_BASE_LAYER == animator.GetCurrentAnimatorStateInfo(0).fullPathHash)
			{
				if (!footsteps.isPlaying)
				{
					footsteps.Play();
				} // if
			} // if
			else
			{
				footsteps.Stop();
			} // else
		} // AudioManagement
		
		void InitData()
		{
			player = GLOBALS.gameController.GetSpawnedPlayer();
			if (null == player)
			{
                General.Logger.LogError("Walk", "InitData", "Could not find player.");
				return;
			} // if
			playerScript = player.GetComponent<Player>();
			if (null == playerScript)
			{
                General.Logger.LogError("Walk", "Awake", "Could not find player's Player script.");
			} // if
			animator = player.GetComponent<Animator>();
			if (null == animator)
			{
                General.Logger.LogError("Walk", "Awake", "Could not find player's animator.");
			} // if
			playerRigidbody = player.GetComponent<Rigidbody>();
			if (null == playerRigidbody)
			{
                General.Logger.LogError("Walk", "Awake", "Could not find player's rigidbody.");
			} // if
			ANIMATION_BASE_LAYER = Animator.StringToHash("Base Layer.Locomotion");
		} // InitData
	} // class
} // namespace