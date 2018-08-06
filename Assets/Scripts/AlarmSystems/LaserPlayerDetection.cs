using UnityEngine;
using System.Collections;

public class LaserPlayerDetection : MonoBehaviour 
{
	private GameObject player;
//	private LastPlayerSighting lastPlayerSighting;

	void Awake()
	{
		player = GameObject.FindWithTag(Tags.player);
//		lastPlayerSighting = GameObject.FindWithTag(Tags.gameController).GetComponent<LastPlayerSighting>();
	} // Awake

	void OnTriggerStay(Collider inOther)
	{
		if (GetComponent<Renderer>().enabled)
		{
			if (inOther.gameObject == player)
			{
//				lastPlayerSighting.position = inOther.transform.position;
			} // if
		} // if
	} // OnTriggerStay
} // class
