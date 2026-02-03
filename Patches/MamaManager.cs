using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Steamworks;


[HarmonyPatch(typeof(MamaManager))]
public class MamaManagerPatch
{
	[HarmonyPatch("OnJumpscareFinished")]
	[HarmonyPrefix]
	static bool Postfix(MamaManager __instance)
	{
		string secondPlayer = SteamLobby.instance.secondPlayerName;
		string currentPlayer = SteamFriends.GetPersonaName();

		var deadplayerList = RevoltMain.instance.deadPlayers;

		if (!deadplayerList.Contains(currentPlayer))
		{
			deadplayerList.Add(currentPlayer);
			SendData.SendPlayerStatus(currentPlayer, false);
		}

		if (deadplayerList.Contains(secondPlayer) &&
			deadplayerList.Contains(currentPlayer)) //everyone ded, restart
		{
			GameManager.OnLevelRestart();
			SendData.SendPlayerStatus(secondPlayer, true);
			SendData.SendPlayerStatus(currentPlayer, true);
			RevoltMain.instance.deadPlayers.Clear(); //clear players, new level
			//RM.flashlight.enabled = true; ?? idk if keep or wut
			FirstPersonControllerPatch.isSpectating = false;
			return false;
		}

		RM.flashlight.flashlight.SetActive(false); //manually set off
		RM.tattletail.tattletail.SetActive(false); //manually set off
		FirstPersonControllerPatch.isSpectating = true;
		RM.hud.SetNewQuest($"Spectating {secondPlayer}");
		return false;
	}
}