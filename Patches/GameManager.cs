using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;


[HarmonyPatch(typeof(GameManager), "OnLevelComplete")] 
public class GameManagerPatch
{
	[HarmonyPostfix]
	static void Postfix(GameManager __instance)
	{
		LevelEnum nextLevel = Utils.GetLevelFlow().GetNextLevel(GD.currentLevel);
		SendData.SendLevelTransition(nextLevel.ToString());
	}
}