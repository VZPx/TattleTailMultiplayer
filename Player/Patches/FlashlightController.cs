using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[HarmonyPatch(typeof(FlashlightController))]
public class FlashlightControllerPatch
{
	[HarmonyPatch("Start")]
	[HarmonyPostfix]
	static void StartPatch(FlashlightController __instance)
	{
		var secondPlayer = OnlinePlayerObject.instance;
		if (secondPlayer.flashLight == null)
			SpawnSecondFlashLight(__instance);
	}
	//spawns second player flashlight
	static void SpawnSecondFlashLight(FlashlightController __instance)
	{
		var secondPlayer = OnlinePlayerObject.instance;
		GameObject secondFlashlight = GameObject.Instantiate(__instance.flashlight,
			secondPlayer.gameObject.transform);
		secondPlayer.flashLight = secondFlashlight;
		secondFlashlight.transform.localEulerAngles = Vector3.zero;
		secondFlashlight.transform.localPosition = new Vector3(0.3477f, 0.4213f, 0.2513f);
		secondFlashlight.transform.localScale = Vector3.one;
		var spotLight = secondFlashlight.transform.FindChild("Flashlight Spotlight")
			.GetComponent<Light>();
		spotLight.intensity = 1f;
		spotLight.range = 7f;
	}

	[HarmonyPatch("SetFlashlight")]
	[HarmonyPostfix] //sets second player flashlight to active or false
	static void SetFlashlightPatch(FlashlightController __instance, bool carry)
	{
		var flashLight = OnlinePlayerObject.instance.flashLight;
		if (flashLight != null)
		{
			flashLight.SetActive(carry);
		}
	}
}