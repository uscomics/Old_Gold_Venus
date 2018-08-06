namespace General
{
	using UnityEngine;
	using System.Collections;

	public class GLOBALS 
	{
		static public int preGame_TotalScore;
		static public int preGame_Coin;
		static public System.DateTime preGame_LastDayPlayed;
		static public int preGame_ConsecutiveDaysPlayingCount;
		static public int preGame_EscapedDeathFactoryCount;
		static public int preGame_KilledInDeathFactoryCount;
		static public int score = 0;
		static public int coin = 0;
		static public System.DateTime lastDayPlayed = System.DateTime.Today;
		static public int consecutiveDaysPlayingCount = 0;
		static public int currentLevel = 0;
		static public int escapedDeathFactoryCount = 0;
		static public int killedInDeathFactoryCount = 0;
		static public GameController gameController = null;
		static public UnityEngine.Vector3 resetPosition = new UnityEngine.Vector3(-1000.0f, -1000.0f, -1000.0f);
		static public UnityEngine.Vector3 lastPlayerSighting = new UnityEngine.Vector3(-1000.0f, -1000.0f, -1000.0f);
	} // class

} // namespace
