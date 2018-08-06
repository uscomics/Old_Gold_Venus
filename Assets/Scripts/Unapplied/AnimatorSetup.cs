using UnityEngine;
using System.Collections;

public class AnimatorSetup
{
	public float speedDampTime = 0.1f;
	public float angularSpeedDampTime = 0.7f;
	public float angleResponseTime = 0.6f;

	private Animator anim;

	public AnimatorSetup(Animator inAnimator)
	{
		anim = inAnimator;
	} // AnimatorSetup

	public void Setup(float inSpeed, float inAngle)
	{
		float angularSpeed = inAngle / angleResponseTime;

		anim.SetFloat(Animator.StringToHash("Speed"), inSpeed, speedDampTime, Time.deltaTime);
		anim.SetFloat(Animator.StringToHash("AngularSpeed"), angularSpeed, angularSpeedDampTime, Time.deltaTime);
	} // Setup
} // class
