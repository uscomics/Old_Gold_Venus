using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour 
{
	public float fadeSpeed = 1.5f;
	public bool sceneStarting = true;

	private Image fader;

	void Awake()
	{
		Canvas c = GetComponent<Canvas>();
		fader = c.GetComponentsInChildren<Image>()[0];
	} // Awake

	void Update()
	{
		if (sceneStarting)
		{
			StartScene();
		} // if
		else
		{
			EndScene();
		} // else
	} // Update
	
	void FadeToClear()
	{
		fader.color = Color.Lerp(fader.color, Color.clear, fadeSpeed * Time.deltaTime);
	} // FadeToClear
	
	void FadeToBlack()
	{
		fader.color = Color.Lerp(fader.color, Color.black, fadeSpeed * Time.deltaTime);
	} // FadeToBlack

	void StartScene()
	{
		FadeToClear();
		if (0.05f >= fader.color.a)
		{
			fader.color = Color.clear;
			fader.enabled = false;
		} // if
	} // StartScene

	public void EndScene()
	{
		fader.enabled = true;
		FadeToBlack();
	} // EndScene
} // class
