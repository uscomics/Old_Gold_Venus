using UnityEngine;
using UnityEngine.UI;
using General;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Drop 
{ 
	public float percentage;
	public GameObject drop;
	public Rect dropRect;
}; // class

[System.Serializable]
public class Hazard 
{
	public GameObject hazardObject;
	public Vector3 hazardPosition;
	public Vector3 hazardRotation = Vector3.zero;
	public Vector3 hazardScale = Vector3.one;
	public int spawnSize = 1;
} // class

[System.Serializable]
public enum LevelChaining
{
	RepeatLevel,
	LoadNextLevel,
	LastLevel,
	EndGame
}; // enum

[System.Serializable]
public enum LevelType
{
	Timed,
	Trigger
}; // enum

[System.Serializable]
public class Level
{ 
	public LevelChaining levelChaining = LevelChaining.LoadNextLevel;
	public LevelType levelType = LevelType.Timed;
	public RawImage openingImage;
	public int openingImageDurationInSeconds = 5;
	public int levelDurationInSeconds;
	public bool spawnPlayer = true;
	public Vector3 spawnPlayerLocation = Vector3.zero;
	public int endLevelWait = 0;
	public AudioSource music;
	public Hazard[] beginningHazards;
	public int spawnWait = 5;
	public Hazard[] spawnedHazards;
	public float dropChance = 0.5f;
	public Drop[] dropTable;

	public bool levelTriggered = false;
}; // class

[System.Serializable]
public class CameraController
{ 
	public Camera levelCamera;
	public float cameraSmoothing = 1.5f;
	public bool trackPlayer = true;

	public float distance = 5.0f;
	public float xSpeed = 30.0f;
	public float ySpeed = 30.0f;
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	public float distanceMin = .5f;
	public float distanceMax = 15f;
	public float smoothTime = 2f;

	private Vector3 relativeCameraPosition;
	private float relativeCameraPositionMagnitude;
	private float rotationYAxis = 0.0f;
	private float rotationXAxis = 0.0f;
	private float velocityX = 0.0f;
	private float velocityY = 0.0f;

	void InitCameraController()
	{
		GameObject mainCamera = GameObject.FindWithTag("MainCamera") as GameObject;
		
		if (null == mainCamera)
		{
			return;
		} // if
		Vector3 angles = mainCamera.transform.eulerAngles;
		rotationYAxis = angles.y;
		rotationXAxis = angles.x;
	} // InitCameraController

	public void UpdateCamera(GameObject inPlayer)
	{
		GameObject mainCamera = GameObject.FindWithTag("MainCamera") as GameObject;
		
		if (null == mainCamera)
		{
			return;
		} // if
		
		if (Input.GetMouseButton(0))
		{
			velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
			velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			
			rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
			
			Quaternion toRotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);
			Quaternion workingRotation = toRotation;
			
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
			
			RaycastHit hit;
			if (Physics.Linecast(inPlayer.transform.position, mainCamera.transform.position, out hit))
			{
				distance -= hit.distance;
			}
			Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
			Vector3 workingPosition = workingRotation * negDistance + inPlayer.transform.position;
			
			mainCamera.transform.rotation = workingRotation;
			mainCamera.transform.position = workingPosition + (inPlayer.transform.up * 2.0f) + (inPlayer.transform.forward * -1.0f);
			
			velocityX = Mathf.Lerp(velocityX, 0, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0, Time.deltaTime * smoothTime);
			return;
		} // if

		if (false == trackPlayer)
		{
			return;
		} // if

		Vector3 playerPosition = inPlayer.transform.position;
		Vector3 behindPlayer = inPlayer.transform.forward * -2.0f;
		Vector3 standardCameraPosition = playerPosition + behindPlayer + (Vector3.up * 2);

		levelCamera.transform.position = Vector3.Lerp(levelCamera.transform.position, standardCameraPosition, cameraSmoothing * Time.deltaTime);
		levelCamera.transform.rotation = Quaternion.Lerp(levelCamera.transform.rotation, inPlayer.transform.rotation, cameraSmoothing * Time.deltaTime);

		Vector3 angles = mainCamera.transform.eulerAngles;

		rotationYAxis = angles.y;
		rotationXAxis = angles.x;
	} // UpdateCamera

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360F)
		{
			angle += 360F;
		} // if
		if (angle > 360F)
		{
			angle -= 360F;
		} // if
		return Mathf.Clamp(angle, min, max);
	} // ClampAngle

}; // class

[System.Serializable]
public class GUIManager
{
	public int maxLives = 7;
	public int currentLives = 3;
	public GameObject healthModel;
	public Vector3 healthModelPosition = Vector3.zero;
	public Vector3 healthModelRotation = Vector3.zero;
	public float healthModelScale = 1.0f;
	public GameObject livesModel;
	public Vector3 livesModelPosition = Vector3.zero;
	public float livesModelScale = 1.0f;
	public float livesModelOffest = 1.0f;
	public Text scoreText;
	public Vector3 scoreTextPosition = Vector3.zero;
	public Button restartButton;
	public Vector3 restartButtonPosition = Vector3.zero;

	private GameObject healthIcon;
	private GameObject[] livesIcons;
	public int originalCurrentLives;

	public void Init()
	{
		if (null != scoreText)
		{
			scoreText.transform.position = scoreTextPosition;
		} // if
		if (null != restartButton)
		{
			restartButton.transform.position = restartButtonPosition;
			restartButton.enabled = false;
			restartButton.image.enabled = false;
			restartButton.GetComponentInChildren<Text>().text = "";
		} // if
		if (null != livesModel)
		{
			livesIcons = new GameObject[maxLives];
			for (int loop = 0; loop < maxLives; loop++)
			{
				float nextY = livesModelPosition.y + (loop * livesModelOffest);
				Vector3 position = new Vector3(livesModelPosition.x, nextY, livesModelPosition.z);
				GameObject livesIcon = GameObject.Instantiate(livesModel, position, Quaternion.Euler(0, 0, 0)) as GameObject;
				
				livesIcon.transform.transform.localScale *= livesModelScale;
				livesIcons[loop] = livesIcon;
			} // for
		} // if
		if (null != healthModel)
		{
			GameObject newHealthIcon = GameObject.Instantiate(healthModel, healthModelPosition, Quaternion.Euler(0, 0, 0)) as GameObject;
			
			newHealthIcon.transform.transform.localScale *= healthModelScale;
			healthIcon = newHealthIcon;
		} // if
		originalCurrentLives = currentLives;
	} // Init

	public void ActivateRestart(bool inActivate = true, string inButtonText = "Restart")
	{
		restartButton.enabled = inActivate;
		restartButton.image.enabled = inActivate;
		restartButton.GetComponentInChildren<Text>().text = inButtonText;
	} // ActivateRestart
	
	public void UpdateScore()
	{
		scoreText.text = "Score: " + GLOBALS.score;
	} // UpdateScore
	
	public void UpdateLives(GameObject inPlayer)
	{
		for (int loop = 0; loop < maxLives; loop++)
		{
			float nextY = livesModelPosition.y + (loop * livesModelOffest);
			Vector3 defaultPosition = inPlayer.transform.position + (inPlayer.transform.forward * -0.4f) + (inPlayer.transform.up * 1.5f);
			Vector3 offsetPosition = defaultPosition + new Vector3(livesModelPosition.x, nextY, livesModelPosition.z);
			GameObject livesIcon = livesIcons[loop];
			
			livesIcon.transform.position = offsetPosition;
			livesIcon.transform.rotation = inPlayer.transform.rotation;
			livesIcon.SetActive(loop < currentLives - 1);
		} // for
	} // UpdateLives

	public void UpdateHealth(GameObject inPlayer)
	{
		Vector3 defaultPosition = inPlayer.transform.position + (inPlayer.transform.forward * -0.4f) + (inPlayer.transform.up * 2.0f);
		Vector3 offsetPosition = defaultPosition + new Vector3(healthModelPosition.x, healthModelPosition.y, healthModelPosition.z);
		Player.Player playerScript = inPlayer.GetComponent<Player.Player>();
		Vector3 localScale = healthIcon.transform.localScale;
		float healthScale = (((float)playerScript.GetCurrentHealth()) / ((float)playerScript.health));

		if (null == playerScript)
		{
            General.Logger.LogError("GUIManager", "UpdateHealth", "Could not find player's player script.");
			return;
		} // if

        General.Logger.LogDetail("GUIManager", "UpdateHealth", "healthScale: " + healthScale + ", playerScript.GetCurrentHealth(): " + playerScript.GetCurrentHealth() + ", playerScript.health: " + playerScript.health + ".");
		healthIcon.transform.position = offsetPosition;
		healthIcon.transform.rotation =  inPlayer.transform.rotation * Quaternion.Euler(healthModelRotation);
		localScale.y = healthModelScale * healthScale;
		healthIcon.transform.localScale = localScale;
	} // UpdateHealth

	public void AddScore(int inNewScoreAmount)
	{
		GLOBALS.score += inNewScoreAmount;
		UpdateScore();
	} // AddScore
	
	public void AddLife(GameObject inPlayer)
	{
		if (currentLives < maxLives)
		{
			currentLives++;
			UpdateLives(inPlayer);
		} // if
	} // AddLife
	
	public void Restart()
	{
		currentLives = originalCurrentLives;
	} // Restart
}; // class

public class GameController : MonoBehaviour 
{
	public CameraController cameraController;
	public GUIManager guiManager;
	public GameObject player;
	public Level[] levels;
	public int gameOverWonLevel;
	public int gameOverLostLevel;

	private bool playerDeath = false;
	private float endTime;
	private GameObject spawnedPlayer;
	private List<GameObject> dynamicallyAllocatedObjects = new List<GameObject>();
	private List<GameObject> dynamicallyAllocatedGUIObjects = new List<GameObject>();
	private SceneFadeInOut sceneFadeInOut;

	void Start()
	{
        General.Logger.LogDetail("GameController", "Start", "Entering Start. guiManager.currentLives = " + guiManager.currentLives);
		GLOBALS.gameController = this;
		if (null == sceneFadeInOut)
		{
			sceneFadeInOut = GetComponentInChildren<SceneFadeInOut>();
			if (null == sceneFadeInOut)
			{
                General.Logger.LogError("GameController", "Start", "Could not find SceneFadeInOut script.");
			} // if
		} // if

		for (int loop = 0; loop < levels.Length; loop++)
		{
			if (null != levels[loop].openingImage)
			{
				levels[loop].openingImage.enabled = false;
			} // if
			if (null != levels[loop].music)
			{
				levels[loop].music.Stop();
			} // if
		} // for

		System.DateTime lastPlayedPlus1Day = GLOBALS.lastDayPlayed.AddDays(1);

		if (lastPlayedPlus1Day == System.DateTime.Today)
		{
			GLOBALS.consecutiveDaysPlayingCount++;
		} // if
		if (0 < guiManager.currentLives)
		{
			RunLevel();
		} // if

		guiManager.Init();
		guiManager.UpdateScore();
		guiManager.UpdateLives(GetSpawnedPlayer());
        General.Logger.LogDetail("GameController", "Start", "Exiting Start.");
	} // Start

	void FixedUpdate()
	{
		if (null == spawnedPlayer)
		{
			return;
		} // if
		cameraController.UpdateCamera(spawnedPlayer);
	} // FixedUpdate

	public void RunLevel()
	{
        General.Logger.LogDetail("GameController", "RunLevel", "Entering RunLevel. GLOBALS.currentLevel = " + GLOBALS.currentLevel + ".");
		if (levels.Length <= GLOBALS.currentLevel)
		{
			return;
		} // if
		StartCoroutine(RunLevelCoroutine());
        General.Logger.LogDetail("GameController", "RunLevel", "Exiting RunLevel.");
	} // RunLevel

	IEnumerator RunLevelCoroutine()
	{
        General.Logger.LogDetail("GameController", "RunLevelCoroutine", "Entering function.");
		int currentLevel = GLOBALS.currentLevel;

		if (levels.Length <= GLOBALS.currentLevel)
		{
            General.Logger.Log("GameController", "RunLevelCoroutine", "GLOBALS.currentLevel " + GLOBALS.currentLevel + " larger than levels.Length " + levels.Length + ". Exiting function.");
			yield break;
		} // if

		sceneFadeInOut.sceneStarting = true;
		endTime = Time.time + levels[GLOBALS.currentLevel].levelDurationInSeconds;
		playerDeath = false;
        General.Logger.Log("GameController", "RunLevelCoroutine", "Starting level " + currentLevel + " with endTime " + endTime + ".");
		if (levels[currentLevel].spawnPlayer)
		{
            General.Logger.Log("GameController", "RunLevelCoroutine", "Spawning player.");
			spawnedPlayer = Instantiate(player, levels[GLOBALS.currentLevel].spawnPlayerLocation, Quaternion.identity) as GameObject;
			dynamicallyAllocatedObjects.Add(spawnedPlayer);
			if (null == spawnedPlayer)
			{
                General.Logger.Log("GameController", "RunLevelCoroutine", "Spawning player failed.");
			} // if
		} // if
        General.Logger.Log("GameController", "RunLevelCoroutine", "Spawning beginningHazards. beginningHazards count is " + levels[currentLevel].beginningHazards.Length + ".");
		for (int loop = 0; loop < levels[currentLevel].beginningHazards.Length; loop++)
		{
			for (int loop2 = 0; loop2 < levels[currentLevel].beginningHazards[loop].spawnSize; loop2++)
			{
				Vector3 spawnPosition = levels[currentLevel].beginningHazards[loop].hazardPosition;
				Vector3 spawnRotation = levels[currentLevel].beginningHazards[loop].hazardRotation;
				Quaternion spawnQuaternion = Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z); 
				GameObject hazard = Instantiate(levels[currentLevel].beginningHazards[loop].hazardObject, spawnPosition, spawnQuaternion) as GameObject;

				hazard.transform.localScale = levels[currentLevel].beginningHazards[loop].hazardScale;
				dynamicallyAllocatedObjects.Add(hazard);
			} // for
		} // for
		if (null != levels[currentLevel].music)
		{
            General.Logger.Log("GameController", "RunLevelCoroutine", "Playing music.");
			levels[currentLevel].music.enabled = true;
			levels[currentLevel].music.Play();
		} // if
		if (null != levels[currentLevel].openingImage)
		{
            General.Logger.Log("GameController", "RunLevelCoroutine", "Displaying opening image.");
			levels[currentLevel].openingImage.enabled = true;
			yield return new WaitForSeconds(levels[currentLevel].openingImageDurationInSeconds);
            General.Logger.Log("GameController", "RunLevelCoroutine", "Hiding opening image.");
			levels[currentLevel].openingImage.enabled = false;
		} // if
		guiManager.UpdateScore();
		guiManager.UpdateLives(GetSpawnedPlayer());
		guiManager.UpdateHealth(GetSpawnedPlayer());

		while (!DidLevelAutoEnd(currentLevel))
		{
			int spawnIndex = 0;

			guiManager.UpdateScore();
			guiManager.UpdateLives(GetSpawnedPlayer());
			guiManager.UpdateHealth(GetSpawnedPlayer());
			while (true)
			{
				bool didSpawn = false;

				for (int loop = 0; loop < levels[currentLevel].spawnedHazards.Length; loop++)
				{
					if (spawnIndex < levels[currentLevel].spawnedHazards[loop].spawnSize)
					{
						Vector3 spawnPosition = levels[currentLevel].spawnedHazards[loop].hazardPosition;
						Vector3 spawnRotation = levels[currentLevel].spawnedHazards[loop].hazardRotation;
						Quaternion spawnQuaternion = Quaternion.Euler(spawnRotation.x, spawnRotation.y, spawnRotation.z); 
						GameObject hazard = Instantiate(levels[currentLevel].spawnedHazards[loop].hazardObject, spawnPosition, spawnQuaternion) as GameObject;

						hazard.transform.localScale = levels[currentLevel].spawnedHazards[loop].hazardScale;
						dynamicallyAllocatedObjects.Add(hazard);
						didSpawn = true;
						yield return new WaitForSeconds(0.5f);
					} // if
				} // for
				if (!didSpawn)
				{
					break; // while
				} // if
				spawnIndex++;
			} // while
			yield return new WaitForSeconds(levels[currentLevel].spawnWait);
		} // while

		sceneFadeInOut.sceneStarting = false;
		if (0 <= levels[currentLevel].endLevelWait)
		{
            General.Logger.Log("GameController", "RunLevelCoroutine", "End level wait: " + levels[currentLevel].endLevelWait + ".");
			yield return new WaitForSeconds(levels[currentLevel].endLevelWait);
		} // if
		if (null != levels[currentLevel].music)
		{
            General.Logger.Log("GameController", "RunLevelCoroutine", "Stopping music.");
			levels[currentLevel].music.Stop();
		} // if
        General.Logger.Log("GameController", "RunLevelCoroutine", "Destroying dynamic objects.");
		DestroyDynamicObjects();

		if (LevelChaining.EndGame != levels[currentLevel].levelChaining)
	    {
		    if ((LevelChaining.LoadNextLevel == levels[currentLevel].levelChaining)
		    && (!playerDeath))
			{
				GLOBALS.currentLevel++;
                General.Logger.Log("GameController", "RunLevelCoroutine", "Incrimented level number to " + GLOBALS.currentLevel + ".");
			} // if
			if (playerDeath)
			{
				levels[GLOBALS.currentLevel].levelChaining = LevelChaining.LoadNextLevel;
				playerDeath = false;
                General.Logger.Log("GameController", "RunLevelCoroutine", "Level chaining and playerDeath reset to LevelChaining.LoadNextLevel after processing player death.");
			} // if
			if (LevelChaining.LastLevel != levels[currentLevel].levelChaining)
			{
                General.Logger.Log("GameController", "RunLevelCoroutine", "Loading level number " + GLOBALS.currentLevel + ".");
				RunLevel();
			} // if
			else if (LevelChaining.LastLevel == levels[currentLevel].levelChaining)
			{
                General.Logger.Log("GameController", "RunLevelCoroutine", "Calling GameOver().");
				GameOver();
			} // else if
		} // if
		else
		{
			guiManager.ActivateRestart();
		} // else

        General.Logger.LogDetail("GameController", "RunLevelCoroutine", "Ending level " + currentLevel);
	} // RunLevelCoroutine

	public void SpawnDrop(Transform inLocation)
	{
        General.Logger.LogDetail("GameController", "SpawnDrop", "Drop location is X: " + inLocation.position.x 
		          + ", Y: " + inLocation.position.y 
		          + ", Z: " + inLocation.position.z + ".");
		int currentLevel = GLOBALS.currentLevel;

        General.Logger.Log("GameController", "SpawnDrop", "Drop chance is " + levels[currentLevel].dropChance + ".");
		if (Random.value <= levels[currentLevel].dropChance)
		{
			float precentageRoll = Random.Range(0, 101);
			Vector2 dropVector = new Vector2(inLocation.position.x, inLocation.position.z);

            General.Logger.Log("GameController", "SpawnDrop", "Drop selector percentage is " + precentageRoll + ".");
			for (int loop = 0; loop < levels[currentLevel].dropTable.Length; loop++)
			{
                General.Logger.Log("GameController", "SpawnDrop", "Drop " + loop + " percentage is " + levels[currentLevel].dropTable[loop].percentage + ".");
                General.Logger.Log("GameController", "SpawnDrop", "Drop " + loop + " dropRect is X: " + levels[currentLevel].dropTable[loop].dropRect.x 
				          + ", Y: " + levels[currentLevel].dropTable[loop].dropRect.y
				          + ", Width: " + levels[currentLevel].dropTable[loop].dropRect.width
				          + ", Height: " + levels[currentLevel].dropTable[loop].dropRect.height + ".");

				if ((precentageRoll <= levels[currentLevel].dropTable[loop].percentage)
				&& (levels[currentLevel].dropTable[loop].dropRect.Contains(dropVector)))
				{
					GameObject drop = Instantiate(levels[currentLevel].dropTable[loop].drop, 
					                              inLocation.position, Quaternion.Euler(-90, -180, 0))
													as GameObject;

					dynamicallyAllocatedObjects.Add(drop);
					break; // for
				} // if
			} // for
		} // if
	} // SpawnDrop
	
	void DestroyDynamicObjects()
	{
		for (int loop = dynamicallyAllocatedObjects.Count - 1; loop >= 0; loop--)
		{
			if (null != dynamicallyAllocatedObjects[loop])
			{
				Destroy(dynamicallyAllocatedObjects[loop]);
				dynamicallyAllocatedObjects.RemoveAt(loop);
			} // if
		} // for
		DestroyDynamicGUIObjects();
	} // DestroyDynamicObjects
	
	void DestroyDynamicGUIObjects()
	{
		for (int loop = dynamicallyAllocatedGUIObjects.Count - 1; loop >= 0; loop--)
		{
			if (null != dynamicallyAllocatedGUIObjects[loop])
			{
				Destroy(dynamicallyAllocatedGUIObjects[loop]);
				dynamicallyAllocatedGUIObjects.RemoveAt(loop);
			} // if
		} // for
	} // DestroyDynamicGUIObjects

	public GameObject GetSpawnedPlayer()
	{
		return spawnedPlayer;
	} // GetSpawnedPlayer

	public void PlayerDeath()
	{
		guiManager.currentLives--;
        General.Logger.Log("GameController", "PlayerDeath", "Decrimented lives count to " + guiManager.currentLives);

		if (0 >= guiManager.currentLives)
		{
            General.Logger.Log("GameController", "PlayerDeath", "Player has no more lives. Ending game.");
			levels[GLOBALS.currentLevel].levelChaining = LevelChaining.EndGame;
			playerDeath = true;
			GameOver();
		} // if
		else
		{
            General.Logger.Log("GameController", "PlayerDeath", "Player has more lives remaining. Resetting current level.");
			endTime = Time.time;
			playerDeath = true;
		} // else
	} // PlayerDeath

	bool DidLevelAutoEnd(int inCurrentLevel)
	{
		if (LevelType.Trigger == levels[inCurrentLevel].levelType)
		{
            General.Logger.LogDetail("GameController", "DidLevelAutoEnd", "levels[inCurrentLevel].levelTriggered || playerDeath: " + (levels[inCurrentLevel].levelTriggered || playerDeath));
			return levels[inCurrentLevel].levelTriggered || playerDeath;
		} // if

        General.Logger.LogDetail("GameController", "DidLevelAutoEnd", "Time.time >= endTime: " + (Time.time >= endTime));
		return Time.time >= endTime;
	} // DidLevelAutoEnd

	void GameOver()
	{
		GLOBALS.coin += Mathf.RoundToInt(GLOBALS.score / 1000);
		if (playerDeath)
		{
            General.Logger.Log("GameController", "GameOver", "Setting level to gameOverLostLevel (" + gameOverLostLevel + ").");
			GLOBALS.currentLevel = gameOverLostLevel;
			GLOBALS.killedInDeathFactoryCount++;
		} // if
		else
		{
            General.Logger.Log("GameController", "GameOver", "Setting level to gameOverWonLevel (" + gameOverWonLevel + ").");
			GLOBALS.currentLevel = gameOverWonLevel;
			GLOBALS.escapedDeathFactoryCount++;
		} // else
        General.Logger.Log("GameController", "GameOver", "Running next level.");
		RunLevel();
	} // GameOver

	public void TriggerLevel()
	{
		if (LevelType.Trigger == levels[GLOBALS.currentLevel].levelType)
		{
			levels[GLOBALS.currentLevel].levelTriggered = true;
		} // if
	} // TriggerLevel

	public void GameRestart()
	{
        General.Logger.LogDetail("GameController", "GameRestart", "Entering function.");
		guiManager.Restart();
		GLOBALS.score = 0;
		GLOBALS.currentLevel = 3;
		Application.LoadLevel(Application.loadedLevelName);
        General.Logger.LogDetail("GameController", "GameRestart", "Exiting function.");
	} // GameRestart
} // class
