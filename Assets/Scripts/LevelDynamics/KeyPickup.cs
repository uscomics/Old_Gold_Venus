using UnityEngine;
using System.Collections;

public class KeyPickup : MonoBehaviour 
{
	public AudioClip keyGrab;

	private GameObject player;
	private Player.Inventory playerInventory;

	void Awake()
	{
		player = GameObject.FindGameObjectWithTag(Tags.player);
		playerInventory = player.GetComponent<Player.Inventory>();
	} // Awake

	void OnTriggerEnter(Collider inOther)
	{
		if (inOther.gameObject == player) 
		{
			AudioSource.PlayClipAtPoint(keyGrab, transform.position);
			playerInventory.hasKey = true;
			Destroy(gameObject);
		} // if
	} // OnTriggerEnter
} // class
