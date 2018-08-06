namespace Player
{
	using UnityEngine;
	using General;
	using System.Collections;

	public class Climb : MonoBehaviour 
	{
		public float climbSpeed = 1.0f;
		public float climbDelay = 1.0f;
		[System.Serializable]
		public enum ClimbingDirection
		{
			NONE,
			UP,
			DOWN,
			DROP
		}; // enum

		private GameObject player;
		private Player playerScript;
		private Animator animator;
		private Rigidbody playerRigidbody;
		private int ANIMATION_SPEED;

		public bool isClimbingDirection(float inHorizontal, float inVertical)
		{
			ClimbingDirection climbingDirection = GetClimbingDirection(inHorizontal, inVertical);

			return ((ClimbingDirection.UP == climbingDirection) || (ClimbingDirection.DOWN == climbingDirection));
		} // isClimbingDirection

		public ClimbingDirection GetClimbingDirection(float inHorizontal, float inVertical)
		{
			ClimbingDirection climbingDirection = ClimbingDirection.NONE;

			if ((0.2f < inHorizontal)
		    && (0.2f < playerRigidbody.transform.forward.x))
			{
				climbingDirection = ClimbingDirection.UP;
			} // if
			else if ((-0.2f > inHorizontal)
		    && (-0.2f > playerRigidbody.transform.forward.x))
			{
				climbingDirection = ClimbingDirection.DOWN;
			} // else if
			else if ((0.2f < inVertical)
		    && (0.2f < playerRigidbody.transform.forward.z))
			{
				climbingDirection = ClimbingDirection.UP;
			} // else if
			else if ((-0.2f > inVertical)
		    && (-0.2f > playerRigidbody.transform.forward.z))
			{
				climbingDirection = ClimbingDirection.DOWN;
			} // else if
			else if ((0 != inVertical)
		    || (0 != inHorizontal))
			{
				climbingDirection = ClimbingDirection.DROP;
			} // else if

			if ((ClimbingDirection.DOWN == climbingDirection)
		    && (0 >= playerRigidbody.position.y))
			{
				climbingDirection = ClimbingDirection.NONE;
			} // if
			if ((ClimbingDirection.DROP == climbingDirection)
		    && (0 >= playerRigidbody.position.y))
			{
				climbingDirection = ClimbingDirection.NONE;
			} // if

            General.Logger.LogDetail("Climb", "GetClimbingDirection", "inHorizontal: " + inHorizontal + ", inVertical: " + inVertical + ", playerRigidbody.transform.forward.x: " + playerRigidbody.transform.forward.x + ", playerRigidbody.transform.forward.z: " + playerRigidbody.transform.forward.z + ".");
            General.Logger.LogDetail("Climb", "GetClimbingDirection", "climbingDirection: " + climbingDirection.ToString() + ".");
			return climbingDirection;
		} // GetClimbingDirection

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
//			float h = Input.GetAxis("Horizontal");
//			float v = Input.GetAxis("Vertical");
//
//			PlayerClimb(h, v);
		} // FixedUpdate

		void PlayerClimb(float inHorizontal, float inVertical)
		{
			ClimbingDirection climbingDirection = GetClimbingDirection(inHorizontal, inVertical);
			float currentClimbSpeed = (ClimbingDirection.UP == climbingDirection)? climbSpeed : -climbSpeed;
			Vector3 currentPosition = playerRigidbody.position;
			float newY = Mathf.Max(0.0f, currentPosition.y + (currentClimbSpeed * Time.deltaTime));
			Vector3 newPosition = new Vector3(currentPosition.x, newY, currentPosition.z);

			animator.SetFloat(ANIMATION_SPEED, climbSpeed, playerScript.movementInfo.speedDampTime, Time.deltaTime);
			playerRigidbody.MovePosition(newPosition);
		} // PlayerClimb

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