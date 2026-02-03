using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[HarmonyPatch(typeof(TattletailController))]
public class TattletailControllerPatch
{
	[HarmonyPatch("Start")]
	[HarmonyPostfix]
	static void StartPatch(TattletailController __instance)
	{
		var secondPlayer = OnlinePlayerObject.instance;
		if (secondPlayer.Tattletail == null)
			SpawnSecondTattleTail(__instance);
	}
	//spawns second player tattletail
	static void SpawnSecondTattleTail(TattletailController __instance)
	{
		var secondPlayer = OnlinePlayerObject.instance;
		GameObject secondTattleTail = GameObject.Instantiate(__instance.tattletail,
			secondPlayer.gameObject.transform);
		secondPlayer.Tattletail = secondTattleTail;
		secondTattleTail.transform.localEulerAngles = new Vector3(337.6473f, 141.1343f, 0f);
		secondTattleTail.transform.localPosition = new Vector3(-0.3094f, 0.1431f, 0.5426f);
		secondTattleTail.transform.localScale = Vector3.one;
	}

	[HarmonyPatch("SetTattletail")]
	[HarmonyPostfix] //sets second player tattletail to active or false
	static void SetTattletailPatch(TattletailController __instance, bool carry, bool silent)
	{
		var tattleTail = OnlinePlayerObject.instance.Tattletail;
		if (tattleTail != null)
		{
			tattleTail.SetActive(carry);
		}
	}
}