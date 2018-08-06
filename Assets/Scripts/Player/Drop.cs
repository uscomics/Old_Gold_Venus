namespace Player
{
	using UnityEngine;
	using General;
	using System.Collections;

	public class Drop : MonoBehaviour 
	{
		public float dropSpeed = 1.0f;
		
		private GameObject player;
		private Player playerScript;
		private Animator animator;
		private Rigidbody playerRigidbody;
		private int ANIMATION_SPEED;
		
		public bool isDroppingDone()
		{
			return (0 >= playerRigidbody.position.y);
		} // isDroppingDone

		void FixedUpdate()
		{
//			if (null == player)
//			{
//				InitData();
//			} // if
//			
//			bool isClimbing = MovementType.CLIMBING == playerScript.movementInfo.movementType;
//			
//			if ((!isClimbing)
//		    || (!ClimbWallBeginDone.isClimbWallBeginAnimationDone))
//			{
//				return;
//			} // if
//
//			PlayerDrop();
		} // FixedUpdate
		
		void PlayerDrop()
		{
			Vector3 currentPosition = playerRigidbody.position;
			float newY = Mathf.Max(0.0f, currentPosition.y - (dropSpeed * Time.deltaTime));
			Vector3 newPosition = new Vector3(currentPosition.x, newY, currentPosition.z);
			float animationSpeed = dropSpeed;

			if (isDroppingDone())
			{
				newPosition.y = 0;
				animationSpeed = 0;
			} // if
			animator.SetFloat(ANIMATION_SPEED, animationSpeed, playerScript.movementInfo.speedDampTime, Time.deltaTime);
			playerRigidbody.MovePosition(newPosition);
		} // PlayerDrop

		void InitData()
		{
			player = GLOBALS.gameController.GetSpawnedPlayer();
			if (null == player)
			{
                General.Logger.LogError("Climb", "InitData", "Could not find player.");
				return;
			} // if
			playerScript = player.GetComponent<Player>();
			if (null == playerScript)
			{
                General.Logger.LogError("Climb", "InitData", "Could not find player's Player script.");
			} // if
			animator = player.GetComponent<Animator>();
			if (null == animator)
			{
                General.Logger.LogError("Climb", "InitData", "Could not find player's animator.");
			} // if
			playerRigidbody = player.GetComponent<Rigidbody>();
			if (null == playerRigidbody)
			{
                General.Logger.LogError("Climb", "InitData", "Could not find player's rigidbody.");
			} // if
			ANIMATION_SPEED = Animator.StringToHash("Speed");
		} // InitData
	} // class
} // namespace
