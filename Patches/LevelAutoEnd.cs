using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;


[HarmonyPatch(typeof(LevelAutoEnd), "WhiteoutRoutine")] 
public class LevelAutoEndPatch
{
	[HarmonyPrefix]
	static bool Prefix(ref IEnumerator __result, LevelAutoEnd __instance)
	{
		__result = PatchedWhiteout(__instance);
		return false; // Skip the original game routine
	}

	static IEnumerator PatchedWhiteout(LevelAutoEnd __instance)
	{
		var trv = Traverse.Create(__instance);
		AudioController.Play("sting_curiosity");
		RM.hud.SetFaderColor(Color.white);
		float timer = trv.Field("whiteOutTimer").GetValue<float>();
		yield return new WaitForSeconds(timer);
		GameManager.PlayLevel(__instance.nextLevel);
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendLevelTransition(__instance.nextLevel.ToString());
		}
		yield break;
	}
}