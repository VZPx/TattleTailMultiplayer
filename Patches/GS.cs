using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using Steamworks;


[HarmonyPatch(typeof(GS))] 
public class GSPatch
{
	[HarmonyPatch("SetupConsoleCommands")]
	[HarmonyPrefix]
	static void Prefix(CheckpointObjectDeactivate __instance)
	{
		GameConsole.AddCallback("discord", new Action(DiscordInvite), "Join our server for more!");
		GameConsole.AddCallback("timeset", new Action<float>(SetTime), "[Sets the scale the game runs in]");
		GameConsole.AddCallback("msg", new Action<string>(MessageAll), "Sends message to all players in lobby");
		GameConsole.AddCallback("debugtest", new Action<string>(DebugWrite), "returns string to console");
		GameConsole.AddCallback("crimsonvision", new Action(ToggleRedMode), "Toggles a red light render perspective");
		GameConsole.AddCallback("restartlevel", new Action(RestartLevel), "Restarts Current Level");
		GameConsole.AddCallback("uncapfps", new Action(UncapFPS), "Plays at unlimited FPS.");
	}

	[HarmonyPatch("GotoLevel")]
	[HarmonyPrefix]
	static bool Prefix(string levelName)
	{
		LevelEnum hashCode = (LevelEnum)levelName.GetHashCode();
		if (levelName == "menu")
		{
			SceneManager.LoadScene("menu", 0);
			return false;
		}
		if (Enum.IsDefined(typeof(LevelEnum), hashCode))
		{
			GameManager.PlayLevel(hashCode);
			SendData.SendLevelTransition(hashCode.ToString());
			return false;
		}
		Debug.LogError("Couldn't find level " + levelName);
		return false;
	}

	public static void UncapFPS()
	{
		Application.targetFrameRate = -1;
		QualitySettings.vSyncCount = 0;
	}

	public static void DiscordInvite()
	{
		Application.OpenURL("https://discord.gg/n8GyE3aJDP");
	}

	public static void ToggleRedMode()
	{
		GS.fullbright = !GS.fullbright;
		GameObject gameObject = GameObject.Find("FullRed");
		Light light = null;
		if (gameObject)
		{
			light = gameObject.GetComponent<Light>();
		}
		if (!GS.fullbright)
		{
			if (light)
			{
				light.gameObject.SetActive(false);
			}
		}
		else if (light)
		{
			light.gameObject.SetActive(true);
		}
		else
		{
			gameObject = new GameObject("FullRed");
			gameObject.transform.SetParent(RM.fpsController.GetComponentInChildren<Camera>().transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			light = gameObject.AddComponent<Light>();
			light.type = LightType.Point;
			light.range = 25f;
			light.intensity = 1f;
			light.color = Color.red;
		}
		Debug.Log("Crimson Vision: " + GS.fullbright.ToString());
	}

	public static void DebugWrite(string info)
	{
		Debug.Log(info);
	}

	private static void SetTime(float time)
	{
		TimeManager.SetTimeScale(time);
	}

	public static void MessageAll(string msg)
	{
		if (SteamLobby.instance != null)
		{
			foreach (string text in msg.Split(new char[] { ' ' }))
			{
				int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.lobby);
				for (int j = 0; j < numLobbyMembers; j++)
				{
					CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobby, j);
					if (lobbyMemberByIndex != SteamUser.GetSteamID())
					{
						SteamLobby.receiver = lobbyMemberByIndex;
						SendData.SendWelcome(SteamFriends.GetPersonaName() + ": " + text);
						DebugWrite(SteamFriends.GetPersonaName() + ": " + text);
					}
				}
			}
			return;
		}
		Debug.Log("SteamLobby instance null!");
	}

	public static void RestartLevel()
	{
		LevelEnum currentLevel = GD.currentLevel;
		if (Enum.IsDefined(typeof(LevelEnum), currentLevel))
		{
			GameManager.PlayLevel(currentLevel);
			SendData.SendLevelTransition(currentLevel.ToString());
			return;
		}
		Debug.LogError("Couldn't find level " + currentLevel);
	}
}

