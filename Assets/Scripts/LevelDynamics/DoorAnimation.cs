using UnityEngine;
using General;
using System.Collections;

public class DoorAnimation : MonoBehaviour
{
	public bool requireKey;
	public AudioClip doorSwishClip;
	public AudioClip accessDeniedClip;

	private Animator anim;
	private Player.Player playerScript;
	private int count;
	private GameController gameControllerScript;

	void Awake()
	{
		anim = GetComponent<Animator>();
	} // Awake

	void OnTriggerEnter(Collider inOther)
	{
        General.Logger.LogDetail("DoorAnimation", "OnTriggerEnter", "Entering function.");
		if (null == gameControllerScript)
		{
			gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
			if (null == gameControllerScript)
			{
                General.Logger.LogError("DoorAnimation", "OnTriggerEnter", "Could not find game controller script. Exiting function.");
				return;
			} // if
		} // if
		if ((requireKey)
	    && (null == playerScript))
		{
			playerScript = gameControllerScript.GetSpawnedPlayer().GetComponent<Player.Player>();
			if (null == playerScript)
			{
                General.Logger.LogError("DoorAnimation", "OnTriggerEnter", "Could not find spawned player's inventory script. Exiting function.");
				return;
			} // if
		} // if
		if (inOther.gameObject == gameControllerScript.GetSpawnedPlayer())
		{
            General.Logger.Log("DoorAnimation", "OnTriggerEnter", "Player triggered door.");
			if (requireKey)
			{
				if (playerScript.inventory.hasKey)
				{
                    General.Logger.Log("DoorAnimation", "OnTriggerEnter", "Player has key.");
					count++;
				} // if
				else
				{
                    General.Logger.Log("DoorAnimation", "OnTriggerEnter", "Player has no key.");
					GetComponent<AudioSource>().clip = accessDeniedClip;
					GetComponent<AudioSource>().Play();
				} // else
			} // if
			else
			{
                General.Logger.Log("DoorAnimation", "OnTriggerEnter", "No key required.");
				count++;
			} // else
		} // if
		else if (inOther.gameObject.tag == Tags.enemy)
		{
            General.Logger.Log("DoorAnimation", "OnTriggerEnter", "Non-Player triggered door.");
			if (inOther is CapsuleCollider)
			{
				count++;
			} // if
		} // else if
        General.Logger.LogDetail("DoorAnimation", "OnTriggerEnter", "Exiting function.");
	} // OnTriggerEnter

	void OnTriggerExit(Collider inOther)
	{
		if (null == gameControllerScript)
		{
			gameControllerScript = GameObject.FindWithTag("GameController").GetComponent<GameController>();
			if (null == gameControllerScript)
			{
                General.Logger.LogError("DoorAnimation", "OnTriggerExit", "Could not find game controller script. Exiting function.");
				return;
			} // if
		} // if
		if ((inOther.gameObject == gameControllerScript.GetSpawnedPlayer())
	    || ((inOther.gameObject.tag == Tags.enemy) && (inOther is CapsuleCollider)))
		{
			if (0 < count)
			{
				count--;
			} // if
		} // if
	} // OnTriggerExit

	void Update()
	{
		anim.SetBool(Animator.StringToHash("Open"), (count > 0));

		if ((anim.IsInTransition(0))
	    && (!GetComponent<AudioSource>().isPlaying))
		{
            General.Logger.Log("DoorAnimation", "Update", "Playing Door Swish clip.");
			GetComponent<AudioSource>().clip = doorSwishClip;
			GetComponent<AudioSource>().Play();
		} // if
	} // Update
} // class
