namespace Player
{
	using UnityEngine;
	using General;
	using System.Collections;
	
	[System.Serializable]
	public class Inventory
	{
		public bool hasKey;
	} // class

	[System.Serializable]
	public enum MovementType
	{
		STANDING = 0,
		WALKING = 1,
		RUNNING = 2,
		DEAD = 3
	} // enum

	[System.Serializable]
	public class MovementInfo
	{
		public float turnSmoothing = 15.0f;
		public float speedDampTime = 0.1f;
		public MovementType movementType = MovementType.STANDING;
	} // class

	public class Player : MonoBehaviour 
	{
		public int health = 100;
		public float resetAfterDeathTime = 5.0f;
		public AudioClip deathClip;
		public Inventory inventory;
		public MovementInfo movementInfo;

		private Animator animator;
		private Walk walkScript;
		private float timer;
		private bool playerDead;
		private int ANIMATION_IS_STANDING;
		private int ANIMATION_IS_WALKING;
		private int ANIMATION_IS_DEAD;
		private ChartData[] chartDataPosition;
		private Chart stateChartPosition;
		private ChartData[] chartDataStandingWalkingDead;
		private Chart stateChartStandingWalkingDead;
		private int dataIndex = 0;
		private int currentHealth;

		void Awake()
		{
			animator = GetComponent<Animator>();
			walkScript = GetComponent<Walk>();
			if (null == walkScript)
			{
                General.Logger.LogError("Player", "Awake", "Could not find player's walk script.");
			} // if
			ANIMATION_IS_STANDING = Animator.StringToHash("IsStanding");
			ANIMATION_IS_WALKING = Animator.StringToHash("IsWalking");
			ANIMATION_IS_DEAD = Animator.StringToHash("IsDead");
			currentHealth = health;

			chartDataPosition = new ChartData[3] {new ChartData("X", 100, new Color(1.0f, 0.5f, 0.0f, 1.0f)), new ChartData("Y", 100, new Color(0.5f, 0.2f, 1.0f, 1.0f)), new ChartData("Z", 100, new Color(1.0f, 0.2f, 0.5f, 1.0f))};
			stateChartPosition = new Chart(chartDataPosition, true, 1000, 150, 100, 7, "Player State Chart 2", "Time", "Position");
			chartDataStandingWalkingDead = new ChartData[3] {new ChartData("Stand", 100, Color.green), new ChartData("Walk", 100, Color.blue), new ChartData("Dead", 100, Color.black)};
			stateChartStandingWalkingDead = new Chart(chartDataStandingWalkingDead, true, 1000, 150, 100, 2, "Player State Chart 3", "Time", "State");
		} // Awake
		
		void FixedUpdate()
		{
			if (playerDead)
			{
				return;
			} // if
			
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis("Vertical");
			
			if ((0.0f == h)
		    && (0.0f == v))
			{
				SetMovementType(MovementType.STANDING);
			} // if
			else
			{
				SetMovementType(MovementType.WALKING);
			} // else
		} // FixedUpdate

		void Update()
		{
			if (currentHealth <= 0.0f)
			{
				if (!playerDead)
				{
					PlayerDying();
				} // if
			} // if
		} // Update

		public void AddKey()
		{
			inventory.hasKey = true;
		} // AddKey
		
		public void AddDime()
		{
			GLOBALS.coin += 10;
		} // AddDime
		
		public void AddHealth()
		{
			currentHealth = health;
		} // AddHealth

		void SetMovementType(MovementType inMovementType)
		{
			if (MovementType.STANDING == inMovementType)
			{
				animator.SetBool(ANIMATION_IS_STANDING, true);
				animator.SetBool(ANIMATION_IS_WALKING, false);
			} // if
			else if (MovementType.WALKING == inMovementType)
			{
				animator.SetBool(ANIMATION_IS_STANDING, false);
				animator.SetBool(ANIMATION_IS_WALKING, true);
			} // else if
			else if (MovementType.DEAD == inMovementType)
			{
				playerDead = true;
				animator.SetBool(Animator.StringToHash("IsWalking"), false);
				animator.SetBool(Animator.StringToHash("IsStanding"), false);
				animator.SetBool(Animator.StringToHash("IsDead"), true);
				if (MovementType.DEAD != movementInfo.movementType)
				{
					GLOBALS.gameController.PlayerDeath();
				} // if
			} // else if
			movementInfo.movementType = inMovementType;
			chartDataPosition[0].data[dataIndex] = GLOBALS.gameController.GetSpawnedPlayer().transform.position.x;
			chartDataPosition[1].data[dataIndex] = GLOBALS.gameController.GetSpawnedPlayer().transform.position.y;
			chartDataPosition[2].data[dataIndex] = GLOBALS.gameController.GetSpawnedPlayer().transform.position.z;
			chartDataStandingWalkingDead[0].data[dataIndex] = System.Convert.ToInt16(animator.GetBool(ANIMATION_IS_STANDING));
			chartDataStandingWalkingDead[1].data[dataIndex] = System.Convert.ToInt16(animator.GetBool(ANIMATION_IS_WALKING));
			chartDataStandingWalkingDead[2].data[dataIndex] = System.Convert.ToInt16(animator.GetBool(ANIMATION_IS_DEAD));
			if (99 == dataIndex)
			{
                General.Logger.LogChart("Player", "SetMovementType", stateChartStandingWalkingDead);
                General.Logger.LogDetail("Player", "SetMovementType", stateChartPosition.ToJSON());
                General.Logger.LogChart("Player", "SetMovementType", stateChartPosition);
                General.Logger.LogScreenShot("Player", "SetMovementType");

				stateChartPosition.ClearData();
				stateChartStandingWalkingDead.ClearData();
				dataIndex = 0;
			} // if
			else
			{
				dataIndex++;
			} // else
		} // SetMovementType

		void PlayerDying()
		{
            General.Logger.LogDetail("Player", "PlayerDying", "Entering function.");
			SetMovementType(MovementType.DEAD);
		} // PlayerDying

		public void TakeDamage(int inDamage)
		{
			currentHealth -= inDamage;
			if (0 > currentHealth)
			{
				currentHealth = 0;
			} // if
		} // TakeDamage
		
		public int GetCurrentHealth()
		{
			return currentHealth;
		} // GetCurrentHealth

	} // class
} // namespace