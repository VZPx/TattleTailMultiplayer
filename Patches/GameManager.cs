using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;


[HarmonyPatch(typeof(GameManager))] 
public class GameManagerPatch
{
	[HarmonyPatch("OnLevelComplete")]
	[HarmonyPrefix]
	static bool Prefix(GameManager __instance)
	{
		Achievements.UnlockAchievementForLevel(GD.currentLevel);
		LevelEnum nextLevel = Utils.GetLevelFlow().GetNextLevel(GD.currentLevel);
		GameManager.PlayLevel(nextLevel);
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendLevelTransition(nextLevel.ToString());
		}
		return false;
	}

	[HarmonyPatch("OnLevelRestart")]
	[HarmonyPostfix]
	static void PostfixRestart(GameManager __instance)
	{
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendLevelTransition(GD.currentLevel.ToString());
		}
	}
}