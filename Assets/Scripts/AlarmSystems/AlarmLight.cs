using UnityEngine;
using System.Collections;

public class AlarmLight : MonoBehaviour 
{
	public float fadeSpeed = 2.0f;
	public float highIntensity = 2.0f;
	public float lowIntensity = 0.5f;
	public float changeMargin = 0.2f;
	public bool alarmOn = false;

	private float targetIntensity;

	void Awake () 
	{
//		GetComponent<Light>().intensity = 0.0f;
//		targetIntensity = highIntensity;
	} // Awake

	void Update()
	{
//		if (true == alarmOn) {
//			GetComponent<Light>().intensity = Mathf.Lerp (GetComponent<Light>().intensity, targetIntensity, fadeSpeed * Time.deltaTime);
//			CheckTargetIntensity ();
//		} // if
//		else 
//		{
//			GetComponent<Light>().intensity = Mathf.Lerp (GetComponent<Light>().intensity, 0, fadeSpeed * Time.deltaTime);
//		} // else
	} // Update

	void CheckTargetIntensity()
	{
//		if (Mathf.Abs(targetIntensity - GetComponent<Light>().intensity) < changeMargin)
//		{
//			if (targetIntensity == highIntensity)
//			{
//				targetIntensity = lowIntensity;
//			} // if
//			else
//			{
//				targetIntensity = highIntensity;
//			} // else
//		} // if
	} // CheckTargetIntensity

} // class
