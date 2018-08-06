using UnityEngine;
using System.Collections;

public class LaserBlinking : MonoBehaviour 
{
	public float onTime;
	public float offTime;

	private float timer;

	void Update()
	{
		timer += Time.deltaTime;
		if ((GetComponent<Renderer>().enabled)
	    && (timer >= onTime)) 
		{
			SwitchBeam();
		} // if
		if ((!GetComponent<Renderer>().enabled)
	    && (timer >= offTime)) 
		{
			SwitchBeam();
		} // if
	} // Update

	void SwitchBeam()
	{
		timer = 0.0f;
		GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
		GetComponent<Light>().enabled = !GetComponent<Light>().enabled;
	} // SwitchBeam
} // class
