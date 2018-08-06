namespace Environment
{
	using UnityEngine;
	using General;
	using System.Collections;

	public class Climbable : MonoBehaviour 
	{
		public static bool canClimb = false;

		void OnTriggerStay(Collider inOther)
		{
			GameObject player = GLOBALS.gameController.GetSpawnedPlayer();
			
			if ((null == player)
		    || (inOther.gameObject != player))
			{
				if ("Key" != inOther.gameObject.tag)
				{
                    General.Logger.LogDetail("Climbable", "OnTriggerStay", "Can't find player or collision with non-player.");
					canClimb = false;
				} // if
				return;
			} // if
			
			if ((1 == inOther.transform.forward.x)
		    || (-1 == inOther.transform.forward.x)
		    || (1 == inOther.transform.forward.z)
		    || (-1 == inOther.transform.forward.z))
			{
				RaycastHit hit;
				
				if (Physics.Raycast(inOther.transform.position + transform.up, inOther.transform.forward, out hit, 1.0f))
				{
					if (hit.collider.gameObject == gameObject)
					{
						canClimb = true;
                        General.Logger.Log("Climbable", "OnTriggerStay", "Player facing climbable surface.");
					} // if
				} // if
			} // if
		} // OnTriggerStay
		
		void OnTriggerExit(Collider inOther)
		{
			GameObject player = GLOBALS.gameController.GetSpawnedPlayer();
			
			if (null == player)
			{
				return;
			} // if
			
			if (inOther.gameObject == player)
			{
                General.Logger.Log("Climbable", "OnTriggerExit", "Not climbing.");
				canClimb = false;
			} // if
		} // OnTriggerExit
	} // class
} // namespace