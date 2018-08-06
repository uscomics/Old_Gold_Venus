using UnityEngine;
using General;
using System.Collections;

public class LiftTrigger : MonoBehaviour 
{
	public float timeToCloseDoors = 2.0f;
	public float timeToLiftStart = 3.0f;
	public float liftSpeed = 3.0f;

	private bool playerInLift = false;
	private float timer = 0.0f;
	private GameController gameControllerScript;

	void OnTriggerEnter(Collider inOther)
	{
        General.Logger.LogDetail("LiftTrigger", "OnTriggerEnter", "Entering function. inOther.tag: " + inOther.tag);
		if (null == gameControllerScript)
		{
			gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
			if (null == gameControllerScript)
			{
                General.Logger.Log("LiftTrigger", "OnTriggerEnter", "gameControllerScript is null. Exiting function.");
				return;
			} // if
		} // if
		if (inOther.gameObject == gameControllerScript.GetSpawnedPlayer())
		{
            General.Logger.Log("LiftTrigger", "OnTriggerEnter", "Player in lift.");
			playerInLift = true;
		} // if
        General.Logger.LogDetail("LiftTrigger", "OnTriggerEnter", "Exiting function.");
	} // OnTriggerEnter
	
	void OnTriggerExit(Collider inOther)
	{
        General.Logger.LogDetail("LiftTrigger", "OnTriggerExit", "Entering function. inOther.tag: " + inOther.tag);
		if (null == gameControllerScript)
		{
			gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
			if (null == gameControllerScript)
			{
                General.Logger.Log("LiftTrigger", "OnTriggerEnter", "gameControllerScript is null. Exiting function.");
				return;
			} // if
		} // if
		if (inOther.gameObject == gameControllerScript.GetSpawnedPlayer())
		{
            General.Logger.Log("LiftTrigger", "OnTriggerEnter", "Player no longer in lift. Resetting timer.");
			playerInLift = false;
			timer = 0.0f;
		} // if
        General.Logger.LogDetail("LiftTrigger", "OnTriggerEnter", "Exiting function.");
	} // OnTriggerExit

	void Update()
	{
		if (null == gameControllerScript)
		{
			gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
			if (null == gameControllerScript)
			{
				return;
			} // if
		} // if

		if (playerInLift)
		{
            General.Logger.Log("LiftTrigger", "Update", "Player in lift. Calling LiftActivation().");
			LiftActivation();
		} // if
	} // Update

	void LiftActivation()
	{
		timer += Time.deltaTime;

		if (timer > timeToLiftStart)
		{
			gameControllerScript.GetSpawnedPlayer().transform.parent = transform;

			transform.Translate(Vector3.up * liftSpeed * Time.deltaTime);
			if (!GetComponent<AudioSource>().isPlaying)
			{
				GetComponent<AudioSource>().Play();
			} // if

			gameControllerScript.TriggerLevel();
		} // if
	} // LiftActivation
} // class
