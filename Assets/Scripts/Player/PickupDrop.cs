namespace Player
{
	using UnityEngine;
	using General;
	using System.Collections;

	public class PickupDrop : MonoBehaviour 
	{
		private GameObject player;
		private Player playerScript;

		void OnTriggerEnter(Collider inOther) 
		{
			if (null == player)
			{
				InitData();
			} // if
			
			if (("ExtraLife" != inOther.tag)
		    && ("Key" != inOther.tag)
		    && ("Dime" != inOther.tag)
			&& ("Health" != inOther.tag))
			{
				return;
			} // if

			if ("ExtraLife" == inOther.tag)
			{
				GLOBALS.gameController.guiManager.AddLife(GLOBALS.gameController.GetSpawnedPlayer());
			} // if
			else if ("Key" == inOther.tag)
			{
				playerScript.AddKey();
			} // else if
			else if ("Dime" == inOther.tag)
			{
				playerScript.AddDime();
			} // else if
			else if ("Health" == inOther.tag)
			{
				playerScript.AddHealth();
			} // else if
			Destroy(inOther.gameObject);
		} // OnTriggerEnter
		
		void InitData()
		{
			player = GLOBALS.gameController.GetSpawnedPlayer();
			if (null == player)
			{
                General.Logger.LogError("PickupDrop", "InitData", "Could not find player.");
				return;
			} // if
			playerScript = player.GetComponent<Player>();
			if (null == playerScript)
			{
                General.Logger.LogError("PickupDrop", "InitData", "Could not find player's Player script.");
			} // if
		} // InitData
	} // class
} // namespace

