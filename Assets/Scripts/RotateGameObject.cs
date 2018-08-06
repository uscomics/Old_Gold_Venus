using UnityEngine;
using System.Collections;

public class RotateGameObject : MonoBehaviour 
{
	public bool rotateX = true;
	public bool rotateY = false;
	public bool rotateZ = false;
	public float speed = 5.0f;

	void Start()
	{
		Rigidbody r = GetComponent<Rigidbody>();
		float x = (rotateX)? r.angularVelocity.x + speed : r.angularVelocity.x;
		float y = (rotateY)? r.angularVelocity.y + speed : r.angularVelocity.y;
		float z = (rotateZ)? r.angularVelocity.z + speed : r.angularVelocity.z;

		r.angularVelocity = new Vector3(x, y, z);
	} // Start
} // class
