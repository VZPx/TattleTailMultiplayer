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
	[HarmonyPostfix]
	static void Postfix(GameManager __instance)
	{
		LevelEnum nextLevel = Utils.GetLevelFlow().GetNextLevel(GD.currentLevel);
		SendData.SendLevelTransition(nextLevel.ToString());
	}
	//test
	[HarmonyPatch("OnLevelRestart")]
	[HarmonyPostfix]
	static void PostfixRestart(GameManager __instance)
	{
		SendData.SendLevelTransition(GD.currentLevel.ToString());
	}
}