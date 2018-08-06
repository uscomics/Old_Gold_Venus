namespace Enemy
{
	using UnityEngine;
	using General;
	using System.Collections;
	
	[System.Serializable]
	public class PatrolInfo
	{
		public Transform[] patrolWayPoints;
		public int wayPointIndex = 0;
		public float deadZone = 5.0f;
		public float patrolTimer = 0.0f;
		public float patrolSpeed = 1.5f;
		public float patrolWaitTime = 0.5f;
		public float speedDampTime = 0.1f;
	} // class
	
	[System.Serializable]
	public class SearchInfo
	{
		public float fieldOfViewAngle = 110.0f;
		public bool playerInSight;
		public Vector3 personalLastSighting;
		public bool playerDidExitTrigger = false;
	} // class
	
	[System.Serializable]
	public class ChaseInfo
	{
		public float sightingPositionMagnitude = 4.0f;
		public float chasingSpeed = 1.5f;
		public float chaseWaitTime = 5.0f;
	} // class
	
	public class PrivateChaseInfo
	{
		public float chaseTimer = 0.0f;
	} // class

	[System.Serializable]
	public class GunInfo
	{
		public int damage = 45;
		public AudioClip shotClip;
		public float flashIntensity = 3.0f;
		public float flashSpeed = 10.0f;
	} // class
	
	public class PrivateGunInfo
	{
		public GameObject gun;
		public Light gunShotLight;
		public bool isShootingAnimationPlaying = false;		
		public bool hasShotBeenFiredForThisClip = false;		
		public int shotCount = 0;
	} // class

	public class AI : MonoBehaviour 
	{
		public PatrolInfo patrolInfo;
		public SearchInfo searchInfo;
		public ChaseInfo chaseInfo;
		public GunInfo gunInfo;

		private PrivateChaseInfo privateChaseInfo = new PrivateChaseInfo();
		private PrivateGunInfo privateGunInfo = new PrivateGunInfo();

		private Animator animator;
		private UnityEngine.AI.NavMeshAgent navMeshAgent;
		private Rigidbody rigidBody;
		private Player.Player playerScript;
		private ChartData[] chartDataPosition;
		private Chart chartPosition;
		private ChartData[] chartDataAnimation;
		private Chart chartAnimation;
		private int dataIndex = 0;
		private int ANIMATION_IS_PATROLLING;
		private int ANIMATION_IS_SHOOTING;
		private int ANIMATION_SHOT;
		private int ANIMATION_SHOT_LIGHT;
		private int ANIMATION_IS_CHASING;

		void Awake()
		{
			animator = GetComponent<Animator>();
			if (null == animator)
			{
                General.Logger.LogError("AI", "Awake", "Could not find enemy's animator.");
			} // if
			navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			if (null == navMeshAgent)
			{
                General.Logger.LogError("AI", "Awake", "Could not find enemy's NavMeshAgent.");
			} // if
			rigidBody = GetComponent<Rigidbody>();
			if (null == rigidBody)
			{
                General.Logger.LogError("AI", "Awake", "Could not find enemy's Rigidbody.");
			} // if
			privateGunInfo.gun = GameObject.FindWithTag("Gun") as GameObject;
			if (null == privateGunInfo.gun)
			{
                General.Logger.LogError("AI", "Awake", "Could not find enemy's gun.");
			} // if
			else
			{
				privateGunInfo.gunShotLight = privateGunInfo.gun.GetComponentInChildren<Light>();
				if (null == privateGunInfo.gunShotLight)
				{
                    General.Logger.LogError("AI", "Awake", "Could not find enemy's gun light.");
				} // if
				else
				{
					privateGunInfo.gunShotLight.enabled = false;
				} // else
			} // else
			chartDataPosition = new ChartData[3] {new ChartData("X", 100, new Color(1.0f, 0.5f, 0.0f, 1.0f)), new ChartData("Y", 100, new Color(0.5f, 0.2f, 1.0f, 1.0f)), new ChartData("Z", 100, new Color(1.0f, 0.2f, 0.5f, 1.0f))};
			chartPosition = new Chart(chartDataPosition, true, 1000, 150, 100, 7, "Enemy State Chart 1: Position", "Time", "Position");
			chartDataAnimation = new ChartData[5] {new ChartData("PlayerInSight", 100, new Color(1.0f, 0.0f, 0.0f, 1.0f)), new ChartData("IsPatrolling", 100, new Color(0.0f, 1.0f, 0.0f, 1.0f)), new ChartData("IsShooting", 100, new Color(0.0f, 0.0f, 1.0f, 1.0f)), new ChartData("Shot", 100, new Color(1.0f, 0.0f, 1.0f, 1.0f)), new ChartData("ShotLight", 100, new Color(0.0f, 1.0f, 1.0f, 1.0f))};
			chartAnimation = new Chart(chartDataAnimation, true, 1000, 150, 100, 7, "Enemy State Chart 2: Animator State", "Time", "State");
			ANIMATION_IS_PATROLLING = Animator.StringToHash("IsPatrolling");
			ANIMATION_IS_SHOOTING = Animator.StringToHash("IsShooting");
			ANIMATION_SHOT = Animator.StringToHash("Shot");
			ANIMATION_SHOT_LIGHT = Animator.StringToHash("ShotLight");
			ANIMATION_IS_CHASING = Animator.StringToHash("IsChasing");
			searchInfo.personalLastSighting = GLOBALS.resetPosition;
			searchInfo.playerInSight = false;		
		} // Awake
		
		void Update()
		{
            General.    Logger.LogDetail("AI", "Update", "Entering function. searchInfo.playerInSight: " + searchInfo.playerInSight + ", searchInfo.personalLastSighting: " + searchInfo.personalLastSighting.ToString() + ", GLOBALS.resetPosition: " + GLOBALS.resetPosition.ToString());

			if (searchInfo.playerInSight)
			{
				if (!animator.GetBool(ANIMATION_IS_SHOOTING))
				{
					StartShooting();
				} // if
				else
				{
					ContinueShooting();
				} // else
			} // if
			else
			{
				if (animator.GetBool(ANIMATION_IS_SHOOTING))
				{
					StopShooting();
				} // if
				StartPatrolling();
			} // else

			chartDataPosition[0].data[dataIndex] = transform.position.x;
			chartDataPosition[1].data[dataIndex] = transform.position.y;
			chartDataPosition[2].data[dataIndex] = transform.position.z;
			chartDataAnimation[0].data[dataIndex] = searchInfo.playerInSight? 4 : 0;
			chartDataAnimation[1].data[dataIndex] = animator.GetBool(ANIMATION_IS_PATROLLING)? 2 : 0;
			chartDataAnimation[2].data[dataIndex] = animator.GetBool(ANIMATION_IS_SHOOTING)? 1 : 0;
			chartDataAnimation[3].data[dataIndex] = animator.GetFloat(ANIMATION_SHOT);
			chartDataAnimation[4].data[dataIndex] = animator.GetFloat(ANIMATION_SHOT_LIGHT);
			if (99 == dataIndex)
			{
                General.Logger.LogDetail("AI", "Update", chartPosition.ToJSON());
                General.Logger.LogChart("AI", "Update", chartPosition);
                General.Logger.LogDetail("AI", "Update", chartAnimation.ToJSON());
                General.Logger.LogChart("AI", "Update", chartAnimation);

				chartPosition.ClearData();
				chartAnimation.ClearData();
				dataIndex = 0;
			} // if
			else
			{
				dataIndex++;
			} // else
		} // Update

		void SetPlayerInSight(bool inSight)
		{
            General.Logger.LogDetail("AI", "SetPlayerInSight", "Entering SetPlayerInSight. inSight = " + inSight);

			searchInfo.playerInSight = inSight;
			searchInfo.playerDidExitTrigger = !inSight;
			if (inSight)
			{
				GLOBALS.lastPlayerSighting = GLOBALS.gameController.GetSpawnedPlayer().transform.position;
				searchInfo.personalLastSighting = GLOBALS.gameController.GetSpawnedPlayer().transform.position;
                General.Logger.LogDetail("AI", "SetPlayerInSight", "Setting personalLastSighting: " + searchInfo.personalLastSighting.ToString());
			} // if
		} // SetPlayerInSight

		void StartPatrolling()
		{
            General.Logger.LogDetail("AI", "StartPatrolling", "Entering function. patrolPath.patrolTimer: " + patrolInfo.patrolTimer + ", navMeshAgent.remainingDistance: " + navMeshAgent.remainingDistance + ", navMeshAgent.stoppingDistance: " + navMeshAgent.stoppingDistance);
			
			float angle = FindRadian(transform.forward, navMeshAgent.desiredVelocity, transform.up);
			
			navMeshAgent.Resume();
			navMeshAgent.speed = patrolInfo.patrolSpeed;
			if (Mathf.Abs(angle) < patrolInfo.deadZone)
			{
				transform.LookAt(transform.position + navMeshAgent.desiredVelocity);
			} // if
			animator.SetBool(ANIMATION_IS_PATROLLING, true);
			
			if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
			{
				patrolInfo.patrolTimer += Time.deltaTime;
				if (patrolInfo.patrolTimer > patrolInfo.patrolWaitTime)
				{
					if (patrolInfo.patrolWayPoints.Length - 1 == patrolInfo.wayPointIndex)
					{
						patrolInfo.wayPointIndex = 0;
					} // if
					else
					{
						patrolInfo.wayPointIndex++;
					} // else
					patrolInfo.patrolTimer = 0.0f;
				} // if
			} // if
			else
			{
				patrolInfo.patrolTimer = 0.0f;
			} // else
			
			navMeshAgent.destination = patrolInfo.patrolWayPoints[patrolInfo.wayPointIndex].position;
            General.Logger.LogDetail("AI", "StartPatrolling", "Exiting function. patrolPath.patrolWayPoints.Length: " + patrolInfo.patrolWayPoints.Length + ", patrolPath.wayPointIndex: " + patrolInfo.wayPointIndex);
		} // StartPatrolling
		
		void StopPatrolling()
		{
            General.Logger.LogDetail("AI", "StopPatrolling", "Entering function.");
			animator.SetBool(ANIMATION_IS_PATROLLING, false);
			navMeshAgent.Stop();
		} // StopPatrolling

		void StartChasing()
		{
            General.Logger.LogDetail("AI", "StartChasing", "Entering function.");

			Vector3 lastSightingLocation = (GLOBALS.resetPosition == searchInfo.personalLastSighting)? GLOBALS.lastPlayerSighting : searchInfo.personalLastSighting;
			Vector3 sightingPositionDelta = lastSightingLocation - transform.position;
			
			animator.SetBool(ANIMATION_IS_CHASING, true);
			if (chaseInfo.sightingPositionMagnitude < sightingPositionDelta.sqrMagnitude)
			{
				navMeshAgent.destination = lastSightingLocation;
			} // if
			
			navMeshAgent.speed = chaseInfo.chasingSpeed;
			
			if (navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)
			{
				privateChaseInfo.chaseTimer += Time.deltaTime;
				if (privateChaseInfo.chaseTimer > chaseInfo.chaseWaitTime)
				{
					searchInfo.personalLastSighting = GLOBALS.resetPosition;
					privateChaseInfo.chaseTimer = 0.0f;
                    General.Logger.LogDetail("AI", "StartChasing", "searchScript.personalLastSighting: " + searchInfo.personalLastSighting.ToString());
				} // if
				if (!searchInfo.playerInSight)
				{
					GLOBALS.lastPlayerSighting = GLOBALS.resetPosition;
				} // if
			} // if
			else
			{
				privateChaseInfo.chaseTimer = 0.0f;
			} // else
            General.Logger.LogDetail("AI", "StartChasing", "Exiting function.");
		} // StartChasing
		
		void StopChasing()
		{
            General.Logger.LogDetail("AI", "StopChasing", "Entering function.");
			animator.SetBool(ANIMATION_IS_CHASING, false);
		} // StopChasing

		/// <summary>
		/// StartShooting, ContinueShooting, StopShooting, OnTriggerEnter, OnTriggerStay, OnTriggerExit,
		/// OnShootingAnimationBegin, and OnShootingAnimationEnd all work together to control shooting.
		/// 
		/// OnTriggerEnter and OnTriggerExit mark when the enemy should begin and end shooting at the player.
		/// However, this combo can be a bit flakey, so the variable searchInfo.playerDidExitTrigger is used
		/// to prevent the OnTriggerExit from having effect until after the shooting animation is done. It is
		/// also used by OnTriggerStay to override the player having been registered as leaving the trigger zone
		/// when she hasn't actually left. 
		/// 
		/// The enemy's rigidbody.isKinematic variable is set to true while the enemy is shooting at the player.
		/// This prevents the enemy's collision detection geometery from slowy pushing the enemy away from the
		/// player while shooting.
		/// 
		/// OnShootingAnimationBegin and OnShootingAnimationEnd mark the beginning and end of a single loop of
		/// shotting animation playing. This in turn is used to drive privateGunInfo.shotCount. The
		/// privateGunInfo.shotCount variable is used to not play the light and sound of a shot for the first
		/// shot. This gives the animation time to properly place the enemy's arm for the shot.
		/// </summary>
		void StartShooting()
		{
            General.Logger.LogDetail("AI", "StartShooting", "Entering function.");

			if (animator.GetBool(ANIMATION_IS_SHOOTING))
			{
				return;
			} // if
			animator.SetBool(ANIMATION_IS_SHOOTING, true);
			rigidBody.isKinematic = true;
			privateGunInfo.shotCount = 0;
            General.Logger.LogDetail("AI", "StartShooting", "Exiting function.");
		} // StartShooting

		void ContinueShooting()
		{
			GameObject player = GLOBALS.gameController.GetSpawnedPlayer();
			float animationShotLight = animator.GetFloat(ANIMATION_SHOT_LIGHT);

			if (1 <= privateGunInfo.shotCount)
			{
				if (animator.GetBool(ANIMATION_IS_PATROLLING))
				{
					StopPatrolling();
				} // if
				if (2 <= privateGunInfo.shotCount)
				{
					privateGunInfo.gunShotLight.enabled = true;
					privateGunInfo.gunShotLight.intensity = gunInfo.flashIntensity * animationShotLight;
					if (!privateGunInfo.hasShotBeenFiredForThisClip)
					{
						if (null == playerScript)
						{
							playerScript = player.GetComponent<Player.Player>();
							if (null == playerScript)
							{
                                General.Logger.LogError("AI", "ContinueShooting", "Could not find player's player script.");
								return;
							} // if
						} // if

						privateGunInfo.hasShotBeenFiredForThisClip = true;
						AudioSource.PlayClipAtPoint(gunInfo.shotClip, privateGunInfo.gunShotLight.transform.position);
						playerScript.TakeDamage(gunInfo.damage);
					} // if
				} // if
			} // if
			transform.LookAt(player.transform.position);
		} // ContinueShooting
		
		void StopShooting()
		{
            General.Logger.LogDetail("AI", "StopShooting", "Entering function.");
			animator.SetBool(ANIMATION_IS_SHOOTING, false);
			privateGunInfo.gunShotLight.intensity = 0.0f;
			rigidBody.isKinematic = false;
		} // StopShooting
		
		void OnTriggerEnter(Collider inOther)
		{
            General.Logger.LogDetail("AI", "OnTriggerEnter", "Entering OnTriggerEnter.");
			GameObject player = GLOBALS.gameController.GetSpawnedPlayer();

			if (null == player)
			{
                General.Logger.LogError("AI", "OnTriggerEnter", "Exiting OnTriggerEnter. Player is null.");
				SetPlayerInSight(false);
				return;
			} // if
			if (inOther.gameObject != player)
			{
                General.Logger.LogDetail("AI", "OnTriggerEnter", "Exiting OnTriggerEnter. Trigger not player. Triggered object's tag is " + inOther.gameObject.tag + ".");
				return;
			} // if
			
			SetPlayerInSight(true);
            General.Logger.LogDetail("AI", "OnTriggerEnter", "Exiting OnTriggerEnter. playerInSight = " + searchInfo.playerInSight);
		} // OnTriggerEnter
		
		void OnTriggerStay(Collider inOther)
		{
			GameObject player = GLOBALS.gameController.GetSpawnedPlayer();
			
			if (null == player)
			{
                General.Logger.LogError("AI", "OnTriggerStay", "Exiting OnTriggerStay. Player is null.");
				SetPlayerInSight(false);
				return;
			} // if
			if (inOther.gameObject != player)
			{
				return;
			} // if
			
			searchInfo.playerDidExitTrigger = false;
		} // OnTriggerStay
		
		void OnTriggerExit(Collider inOther)
		{
            General.Logger.LogDetail("AI", "OnTriggerExit", "Entering OnTriggerExit. inOther.gameObject.tag is " + inOther.gameObject.tag + ".");
			GameObject player = GLOBALS.gameController.GetSpawnedPlayer();
			
			if (null == player)
			{
                General.Logger.LogError("AI", "OnTriggerExit", "Could not find player game object.");
				SetPlayerInSight(false);
				return;
			} // if
			
			if (inOther.gameObject != player)
			{
                General.Logger.LogDetail("AI", "OnTriggerExit", "Trigger not player. Triggered object's tag is " + inOther.gameObject.tag + ".");
				return;
			} // if

            General.Logger.LogDetail("AI", "OnTriggerExit", "Player left trigger zone");
			searchInfo.playerDidExitTrigger = true;
            General.Logger.LogDetail("AI", "OnTriggerExit", "Exiting OnTriggerExit.");
		} // OnTriggerExit

		void OnShootingAnimationBegin()
		{
            General.Logger.LogDetail("AI", "OnStopShooting", "Entering function.");
			privateGunInfo.isShootingAnimationPlaying = true;
			privateGunInfo.hasShotBeenFiredForThisClip = false;
		} // OnShootingAnimationBegin

		void OnShootingAnimationEnd()
		{
            General.Logger.LogDetail("AI", "OnStopShooting", "Entering function.");
			privateGunInfo.isShootingAnimationPlaying = false;
			privateGunInfo.shotCount++;
			if (searchInfo.playerDidExitTrigger)
		    {
				SetPlayerInSight(false);
				searchInfo.playerDidExitTrigger = false;
				privateGunInfo.shotCount = 0;
			} // if
		} // OnShootingAnimationEnd

		float FindRadian(Vector3 inFrom, Vector3 inTo, Vector3 inUp)
		{
			if (inTo == Vector3.zero)
			{
				return 0.0f;
			} // if
			
			float angle = Vector3.Angle(inFrom, inTo);
			Vector3 normal = Vector3.Cross(inFrom, inTo);
			
			angle *= Mathf.Sign(Vector3.Dot(normal, inUp));
			angle *= Mathf.Deg2Rad;
			
			return angle;
		} // FindRadian
		
	} // class
} // namespace