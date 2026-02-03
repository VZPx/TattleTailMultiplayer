using BepInEx;
using HarmonyLib;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[HarmonyPatch(typeof(FirstPersonController))]
public class FirstPersonControllerPatch
{
	public static bool isDebug = false;
	public static bool isSpectating = false;

	[HarmonyPatch("UpdateProcessingProfile")]
	[HarmonyPostfix]
	static void Postfix(FirstPersonController __instance)
	{
		string ownerName = SteamFriends.GetPersonaName();
		if (ownerName == "VZP" || ownerName == "SonicSegaMan")
		{
			GSPatch.DebugMode();
		}
	}

	static void Spectate(FirstPersonController __instance)
	{
		var trv = Traverse.Create(__instance);
		var camera = trv.Field("m_Camera").GetValue<Camera>();

		//Player is Spectating!
		__instance.LockPlayerControls(); //spectator can't move

		var secondPlayer = OnlinePlayerObject.instance;

		if (secondPlayer == null)
		{
			isSpectating = false;
			Debug.LogError("Second player null!");
			return;
		}

		GameObject camTransform = secondPlayer.CamTransform;

		if (camTransform == null)
		{
			//Firstperson angle
			camTransform = new GameObject("CAMTRANSFORM");
			camTransform.transform.parent = secondPlayer.transform;
			camTransform.transform.localPosition = new Vector3(0, 0.7491f, 0);
			camTransform.transform.localEulerAngles = Vector3.zero;
			secondPlayer.CamTransform = camTransform;
		}

		//thirdperson angle
		//camTransform.transform.localPosition = new Vector3(0.7309f, 0.9863f, -1.9491f);
		//camTransform.transform.localEulerAngles = new Vector3(0, 0, 0.0764f);

		camera.transform.position = camTransform.transform.position;
		//camera.transform.parent = secondPlayer.transform;
		camera.transform.rotation = camTransform.transform.rotation;

		var deadplayerList = RevoltMain.instance.deadPlayers;

		if (deadplayerList == null)
		{
			isSpectating = false;
			Debug.LogError("deadplayerList null!");
			return;
		}

		if (!isDebug)
		{
			isSpectating = deadplayerList.Contains(SteamFriends.GetPersonaName());

			if (!isSpectating)
			{
				__instance.UnlockPlayerControls();
			}
		}
	}

	[HarmonyPatch("Update")]
	[HarmonyPrefix]
	static bool UpdatePatch(FirstPersonController __instance)
	{
		if (isSpectating)
		{
			Spectate(__instance);
			return false;
		}
		return true;
	}
}