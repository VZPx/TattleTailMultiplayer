using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;


[HarmonyPatch(typeof(GameConsole), "AddCallbacks")] 
public class GameConsolePatch
{
	[HarmonyPostfix]
	static void Postfix(CheckpointObjectDeactivate __instance)
	{
		GameConsole.AddCallback("mp", new Action(MP_Features), "Multiplayer Features");
	}
	private static void MP_Features()
	{
		Debug.Log("MP features are accessible through the Main Menu");
	}
}