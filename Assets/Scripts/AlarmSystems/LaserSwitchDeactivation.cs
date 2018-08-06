using UnityEngine;
using System.Collections;

public class LaserSwitchDeactivation : MonoBehaviour 
{
	public GameObject laser;
	public Material unlockedMaterial;

	private GameObject player;
	private bool isActive = true;

	void Awake()
	{
		player = GameObject.FindWithTag(Tags.player);
	} // Awake

	void OnTriggerStay(Collider inOther)
	{
		if (inOther.gameObject == player)
		{
			LaserDeactivation();
		} // if
	} // OnTriggerStay

	void LaserDeactivation()
	{
		Renderer screen = transform.Find("prop_switchUnit_screen").GetComponent<Renderer>();

		laser.SetActive(false);
		if (true == isActive)
		{
			GetComponent<AudioSource>().Play();
			screen.material = unlockedMaterial;
			isActive = false;
		} // if
	} // LaserDeactivation
} // class
