using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections.Generic;
using System;

[HarmonyPatch(typeof(MainMenu))]
public class MainMenuPatch
{
	// We use these to store the new UI elements we create since they aren't in the original class
	public static GameObject MPHolder;
	public static GameObject mainPanel;
	public static GameObject multiplayer;
	public static SuperTextMesh ConnectionsMP;
	public static SuperTextMesh MPVersion;
	public static GameObject inviteFriendsButton;
	public static GameObject mamaEvil;
	public static Quaternion mamaRotation = Quaternion.identity;
	public static bool mamaFound;
	public static string GameVersion;

	public static MainMenu instance;

	public static void MultiplayerSupport(MainMenu __instance)
	{
		mamaRotation = Quaternion.Euler(0f, 180f, 0f);
		AudioController.Play("vhs_cam_change", Camera.main.transform);
		__instance.memoriesContent.gameObject.SetActive(false);
		__instance.eggGroup.gameObject.SetActive(false);
		__instance.wipeDataMenuHolder.SetActive(false);
		__instance.optionsMenuHolder.SetActive(false);
		if (MPHolder != null)
		{
			MPHolder.SetActive(true);
		}
		__instance.mainMenuHolder.SetActive(false);
		__instance.memoriesContent.gameObject.SetActive(false);
		var trv = Traverse.Create(__instance);
		trv.Method("RebuildText").GetValue();
	}

	private static void CreateLobby()
	{
		AudioController.Play("vhs_glitch", Camera.main.transform);
		SteamLobby.instance.CreateLobby();
	}

	private static void MP_OnBack(MainMenu __instance)
	{
		AudioController.Play("vhs_cam_change", Camera.main.transform);
		mamaRotation = Quaternion.Euler(0f, 0f, 0f);
		var trv = Traverse.Create(__instance);
		trv.Method("SetMainMenuActive").GetValue();
	}

	[HarmonyPatch("Start")]
	[HarmonyPrefix]
	static bool StartPrefix(MainMenu __instance)
	{
		instance = __instance;

		if (__instance.gameObject.GetComponent<MainMenuMPHelper>() == null)
		{
			__instance.gameObject.AddComponent<MainMenuMPHelper>();
		}

		var trv = Traverse.Create(__instance);

		GameVersion = "0.9.6 [Quest Interactables?]";
		Application.runInBackground = true;
		if (!(GameObject.Find("NetworkManager") != null))
		{
			GameObject networkManager = new GameObject("NetworkManager");
			networkManager.AddComponent<SteamLobby>();
			networkManager.AddComponent<RevoltMain>();
			//networkManager.AddComponent<GHConsole>();
			global::UnityEngine.Object.DontDestroyOnLoad(networkManager);
		}
		trv.Method("SetMainMenuActive").GetValue();
		for (int i = 0; i < __instance.txts.Length; i++)
		{
			__instance.txts[i].gameObject.SetActive(false);
		}
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		global::UnityEngine.Object.FindObjectOfType<VerticalLayoutGroup>().padding.top = -80;
		foreach (GameObject gameObject in global::UnityEngine.Object.FindObjectsOfType<GameObject>())
		{
			if (gameObject.name == "Button_Quit")
			{
				GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject.transform.GetChild(0).gameObject, mainPanel.transform.parent);
				gameObject2.transform.localPosition = new Vector3(109.1346f, 99.2399f, 0f);
				gameObject2.GetComponent<SuperTextMesh>().text = "MP Mod " + GameVersion;
				gameObject2.GetComponent<SuperTextMesh>().enabled = true;
				MPVersion = gameObject2.GetComponent<SuperTextMesh>();
				MPVersion.color = Color.yellow;
				gameObject2.name = "MPVersion";
				gameObject2.SetActive(true);
				global::UnityEngine.Object.Instantiate<GameObject>(gameObject, mainPanel.transform);
				multiplayer = GameObject.Find("Button_Quit(Clone)");
				multiplayer.transform.GetChild(0).gameObject.SetActive(true);
				multiplayer.transform.SetAsFirstSibling();
				multiplayer.transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Multiplayer";
				multiplayer.transform.GetChild(0).GetComponent<SuperTextMesh>().enabled = true;
				Array.Resize<SuperTextMesh>(ref __instance.txts, __instance.txts.Length + 1);
				__instance.txts[25] = multiplayer.transform.GetChild(0).GetComponent<SuperTextMesh>();
				multiplayer.name = "Button_MP";
				multiplayer.GetComponent<Button>().onClick = null;
				multiplayer.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
				multiplayer.GetComponent<Button>().onClick.AddListener(()=> { MultiplayerSupport(__instance); });
				GameObject.Find("Button_LevelSelect").transform.SetAsFirstSibling();
				trv.Method("RebuildText").GetValue();
			}
			else if (gameObject.name == "MainPanel")
			{
				mainPanel = gameObject;
				global::UnityEngine.Object.Instantiate<GameObject>(gameObject, mainPanel.transform.parent);
				Array.Resize<SuperTextMesh>(ref __instance.txts, __instance.txts.Length + 6);
				MPHolder = GameObject.Find("MainPanel(Clone)");
				MPHolder.name = "MP_Holder";
				MPHolder.gameObject.SetActive(false);
				MPHolder.transform.FindChild("Button_LevelSelect").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "--Multiplayer MOD--\n      by VZP";
				__instance.txts[26] = MPHolder.transform.FindChild("Button_LevelSelect").transform.GetChild(0).GetComponent<SuperTextMesh>();
				global::UnityEngine.Object.Destroy(MPHolder.transform.FindChild("Button_LevelSelect").GetComponent<Button>());
				global::UnityEngine.Object.Destroy(MPHolder.transform.FindChild("Button_LevelSelect").GetComponent<Image>());
				MPHolder.transform.FindChild("Button_LevelSelect").gameObject.name = "MP_Header";
				MPHolder.transform.FindChild("Button_Quit").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Create Lobby";
				__instance.txts[27] = MPHolder.transform.FindChild("Button_Quit").transform.GetChild(0).GetComponent<SuperTextMesh>();
				MPHolder.transform.FindChild("Button_Quit").GetComponent<Button>().onClick = null;
				MPHolder.transform.FindChild("Button_Quit").GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
				MPHolder.transform.FindChild("Button_Quit").GetComponent<Button>().onClick.AddListener(new UnityAction(CreateLobby));
				MPHolder.transform.FindChild("Button_Quit").gameObject.name = "CreateLobby";
				SteamLobby.instance.CheckIfLobby();
				MPHolder.transform.FindChild("Button_Continue").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Invite Friend";
				__instance.txts[28] = MPHolder.transform.FindChild("Button_Continue").transform.GetChild(0).GetComponent<SuperTextMesh>();
				MPHolder.transform.FindChild("Button_Continue").GetComponent<Button>().onClick = null;
				MPHolder.transform.FindChild("Button_Continue").GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
				MPHolder.transform.FindChild("Button_Continue").GetComponent<Button>().onClick.AddListener(new UnityAction(SteamLobby.instance.InviteFriends));
				inviteFriendsButton = MPHolder.transform.FindChild("Button_Continue").gameObject;
				inviteFriendsButton.name = "InviteFriend";
				inviteFriendsButton.GetComponent<Button>().interactable = false;
				MPHolder.transform.FindChild("Button_NewGame").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Connected: " + SteamLobby.instance.secondPlayerName;
				ConnectionsMP = MPHolder.transform.FindChild("Button_NewGame").transform.GetChild(0).GetComponent<SuperTextMesh>();
				__instance.txts[29] = MPHolder.transform.FindChild("Button_NewGame").transform.GetChild(0).GetComponent<SuperTextMesh>();
				global::UnityEngine.Object.Destroy(MPHolder.transform.FindChild("Button_NewGame").GetComponent<Button>());
				global::UnityEngine.Object.Destroy(MPHolder.transform.FindChild("Button_NewGame").GetComponent<Image>());
				MPHolder.transform.FindChild("Button_NewGame").gameObject.name = "Connections";
				MPHolder.transform.FindChild("Connections").transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta += new Vector2(6f, 0f);
				MPHolder.transform.FindChild("Connections").transform.GetChild(0).gameObject.GetComponent<RectTransform>().position += new Vector3(0.1f, 0f, 0f);
				MPHolder.transform.FindChild("Button_Options").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Back";
				__instance.txts[30] = MPHolder.transform.FindChild("Button_Options").transform.GetChild(0).GetComponent<SuperTextMesh>();
				MPHolder.transform.FindChild("Button_Options").GetComponent<Button>().onClick = null;
				MPHolder.transform.FindChild("Button_Options").GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
				MPHolder.transform.FindChild("Button_Options").GetComponent<Button>().onClick.AddListener(() => { MP_OnBack(__instance); });
				MPHolder.transform.FindChild("Button_Options").gameObject.name = "Back_ToMainMenu";
				MPHolder.transform.FindChild("Back_ToMainMenu").transform.SetAsLastSibling();
				foreach (object obj in gameObject.transform)
				{
					((Transform)obj).gameObject.SetActive(true);
				}
				foreach (object obj2 in MPHolder.transform)
				{
					((Transform)obj2).gameObject.SetActive(true);
				}
			}
		}

		trv.Method("RebuildText").GetValue();
		return false; // Skip original Start
	}
	

	[HarmonyPatch("SetMainMenuActive")]
	[HarmonyPrefix]
	static bool SetMainMenuActivePatch(MainMenu __instance)
	{
		var trv = Traverse.Create(__instance);
		__instance.wipeDataMenuHolder.SetActive(false);
		__instance.optionsMenuHolder.SetActive(false);
		__instance.mainMenuHolder.SetActive(true);
		__instance.levelSelectHolder.SetActive(false);
		__instance.buttonContinue.gameObject.SetActive(GD.currentLevel > LevelEnum.None);
		__instance.buttonLevelSelect.gameObject.SetActive(GD.HasCompletedGame());
		__instance.memoriesContent.gameObject.SetActive(GD.WaydriveContentUnlocked());
		if (MPHolder != null)
		{
			MPHolder.SetActive(false);
		}
		__instance.eggGroup.gameObject.SetActive(true);
		trv.Method("RebuildText").GetValue();
		return false;
	}

	[HarmonyPatch("SetOptionsActive")]
	[HarmonyPrefix]
	static bool SetOptionsActivePatch(MainMenu __instance)
	{
		__instance.wipeDataMenuHolder.SetActive(false);
		__instance.mainMenuHolder.SetActive(false);
		__instance.memoriesContent.gameObject.SetActive(false);
		__instance.optionsMenuHolder.SetActive(true);
		if (MPHolder != null)
		{
			MPHolder.SetActive(false);
		}
		__instance.levelSelectHolder.SetActive(false);

		var trv = Traverse.Create(__instance);
		trv.Method("RefreshSensitivityText").GetValue();
		trv.Method("RefreshMotionBlurText").GetValue();
		trv.Method("RefreshAAText").GetValue();
		trv.Method("RefreshAOText").GetValue();
		trv.Method("RefreshInvertText").GetValue();
		trv.Method("RebuildText").GetValue();
		return false;
	}

	[HarmonyPatch("SetLevelSelectActive")]
	[HarmonyPrefix]
	static bool SetLevelSelectActivePatch(MainMenu __instance)
	{
		__instance.wipeDataMenuHolder.SetActive(false);
		__instance.mainMenuHolder.SetActive(false);
		__instance.memoriesContent.gameObject.SetActive(false);
		__instance.optionsMenuHolder.SetActive(false);
		if (MPHolder != null)
		{
			MPHolder.SetActive(false);
		}
		__instance.levelSelectHolder.SetActive(true);
		var trv = Traverse.Create(__instance);
		trv.Method("RebuildText").GetValue();
		return false;
	}

	[HarmonyPatch("SetWipeDataActive")]
	[HarmonyPrefix]
	static bool SetWipeDataActivePatch(MainMenu __instance)
	{
		__instance.wipeDataMenuHolder.SetActive(true);
		__instance.optionsMenuHolder.SetActive(false);
		if (MPHolder != null)
		{
			MPHolder.SetActive(false);
		}
		__instance.mainMenuHolder.SetActive(false);
		__instance.memoriesContent.gameObject.SetActive(false);
		var trv = Traverse.Create(__instance);
		trv.Method("RebuildText").GetValue();
		return false;
	}

	[HarmonyPatch("RebuildText")]
	[HarmonyPrefix]
	static bool RebuildTextPatch(MainMenu __instance)
	{
		for (int i = 0; i < __instance.txts.Length; i++)
		{
			if (__instance.txts[i] != null)
			{
				if (__instance.txts[i].transform.parent.gameObject.activeInHierarchy
				&& __instance.txts[i] != null)
				{
					__instance.txts[i].gameObject.SetActive(true);
					__instance.txts[i].text = __instance.txts[i].text;
					__instance.txts[i].Rebuild();
					if (MPVersion)
					{
						MPVersion.Rebuild();
					}
				}
			}
		}
		return false;
	}

	[HarmonyPatch("OnPressButtonContinue")]
	[HarmonyPrefix]
	static bool ContinuePrefix()
	{
		GameManager.PlayLevel(GD.currentLevel);
		SendData.SendLevelTransition(GD.currentLevel.ToString());
		return false;
	}

	[HarmonyPatch("OnPressButtonNewGame")]
	[HarmonyPrefix]
	static bool NewGamePrefix()
	{
		GameManager.PlayLevel((LevelEnum)Utils.GetLevelFlow().levels[0].name.GetHashCode());
		SendData.SendLevelTransition(Utils.GetLevelFlow().levels[0].name.GetHashCode().ToString());
		return false;
	}

	[HarmonyPatch("OnPressLevelSelectButton")]
	[HarmonyPrefix]
	static bool LevelSelect(int levelIndex)
	{
		GameManager.PlayLevel((LevelEnum)Utils.GetLevelFlow().levels[levelIndex].name.GetHashCode());
		SendData.SendLevelTransition(Utils.GetLevelFlow().levels[levelIndex].name.GetHashCode().ToString());
		return false;
	}

	[HarmonyPatch("OnPressButtonMemories")]
	[HarmonyPrefix]
	static bool Memories()
	{
		GameManager.PlayLevel(LevelEnum._Memory_01_Christmas);
		SendData.SendLevelTransition(LevelEnum._Memory_01_Christmas.ToString());
		return false;
	}

}