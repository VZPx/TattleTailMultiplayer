using HarmonyLib;
using Steamworks;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleData
{
	public static bool isNetworkPacket = false;

	public static void WelcomeReceived(Packet _packet)
	{
		Debug.Log(_packet.ReadString());
	}

	public static void MovementReceived(Packet _packet)
	{
		Movement movement = OnlinePlayerObject.instance._Movement;
		movement._PlayerObjectPosition = _packet.ReadVector3();
		movement._PlayerObjectRotation = _packet.ReadFloat();
		movement._PlayerObjectSpeed = _packet.ReadVector3();
	}

	public static void InteractionReceived(Packet _packet)
	{
		Debug.Log("Interaction Received");
		Vector3 origin = _packet.ReadVector3();
		Vector3 direction = _packet.ReadVector3();
		bool fromAutoComplete = _packet.ReadBool();
		string name = _packet.ReadString();
		LayerMask layerMask = -1;
		if (Physics.SphereCast(origin, 0.5f, direction, out var hitInfo, 2f, layerMask))
		{
			Interactable component = hitInfo.transform.GetComponent<Interactable>();
			if (component != null)
			{
				Debug.Log("Interaction from " + component.gameObject.name);
				component.OnInteractComplete(fromAutoComplete);
				return;
			}
			try
			{
				Interactable interactable = (GameObject.Find(name).GetComponent<Interactable>() ? GameObject.Find(name).GetComponent<Interactable>() : GameObject.Find(name).GetComponentInChildren<Interactable>());
				interactable.OnInteractComplete(fromAutoComplete);
				Debug.Log("Tried new method of " + interactable.gameObject.name);
				return;
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
				return;
			}
		}
		try
		{
			Interactable interactable2 = (GameObject.Find(name).GetComponent<Interactable>() ? GameObject.Find(name).GetComponent<Interactable>() : GameObject.Find(name).GetComponentInChildren<Interactable>());
			interactable2.OnInteractComplete(fromAutoComplete);
			Debug.Log("Tried new method of " + interactable2.gameObject.name);
		}
		catch (Exception ex2)
		{
			Debug.Log(ex2.Message);
		}
	}

	public static void InputReceived(Packet _packet)
	{
	}

	public static void LevelTransition(Packet _packet)
	{
		isNetworkPacket = true;
		//Main.Logger.LogInfo($"Got to level: {_packet.ReadString()}");
		GameManager.PlayLevel(GetEnumValue(_packet.ReadString()));
		isNetworkPacket = false;
	}

	private static LevelEnum GetEnumValue(string input)
	{
		try
		{
			return (LevelEnum)Enum.Parse(typeof(LevelEnum), input, ignoreCase: true);
		}
		catch (ArgumentException)
		{
			throw new ArgumentException("Invalid enum value: " + input);
		}
	}

	private static string GetLevelId()
	{
		LevelData levelByEnum = Utils.GetLevelFlow().GetLevelByEnum(GD.currentLevel);
		string id = levelByEnum.secondarySceneName;
		Debug.Log($"Returning: {id}");
		return id;
	}

	public static void CommericalAction(Packet _packet)
	{
		string text = _packet.ReadString();
		Debug.Log("Changed camera!");
		var commercialDirector = UnityEngine.Object.FindObjectOfType<CommercialDirectorPlayable>();
		if (commercialDirector != null)
		{
			isNetworkPacket = true;
			if (text == "")
			{
				commercialDirector.OnHitPlayButton();
			}
			else
			{
				commercialDirector.TryPlayShot(text);
			}
			isNetworkPacket = false;
		}
	}

	public static void HandleSwap(Packet _packet)
	{
		string name = _packet.ReadString();
		Vector3 position = _packet.ReadVector3();
		Quaternion rotation = _packet.ReadQuaternion();
		GameObject.Find(name).transform.position = position;
		GameObject.Find(name).transform.rotation = rotation;
	}

	public static void HasInputtedItem(Packet _packet)
	{
		bool hasInputtedValue = _packet.ReadBool();

		HUDPatch.hasInputted = hasInputtedValue;
	}

	public static void HandleTrigger(Packet _packet)
	{
		string name = _packet.ReadString();
		bool flag = _packet.ReadBool();
		bool fromAutoComplete = _packet.ReadBool();
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null) //gObj is probably in X_scene (used for level specific quests)
		{
			gameObject = FindObjectInScene($"{GetLevelId()}", name);
		}
		if (gameObject != null)
		{
			Trigger component = gameObject.GetComponent<Trigger>();
			if (component != null)
			{
				isNetworkPacket = true;

				if (!flag)
				{
					component.CallNormalTriggerMP();
				}
				else
				{
					component.ManualTrigger(fromAutoComplete);
				}
				isNetworkPacket = false;
			}
			Debug.Log($"Trigger: {name} " +
				$"called -MP");
		}
		else
		{
			Debug.LogError($"Trigger: {name}, was not found -MP");
		}
	}

	public static void HandleQuestTrigger(Packet _packet)
	{
		string name = _packet.ReadString();
		bool autoComplete = _packet.ReadBool();
		bool questComplete = _packet.ReadBool();
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null) //gObj is probably in X_scene (used for level specific quests)
		{
			gameObject = FindObjectInScene($"{GetLevelId()}", name);
		}
		if (gameObject != null)
		{
			QuestTrigger questTrigger = gameObject.GetComponent<QuestTrigger>();
			if (questTrigger != null)
			{
				isNetworkPacket = true;

				if (autoComplete || (!autoComplete && !questComplete))
				{
					RM.questOrder.CompleteCurrentQuest(gameObject);
				}
				else
				{
					questTrigger.CompleteQuest();
				}
				Debug.Log($"Quest Trigger: {name}, called! -MP");
				isNetworkPacket = false;

			}
		}
		else
		{
			Debug.LogError($"Quest Trigger: {name}, was not found -MP");
		}
	}

	public static void HandlePlayerStatus(Packet _packet)
	{
		string playerName = _packet.ReadString();
		bool isAlive = _packet.ReadBool();

		var deadPlayerList = RevoltMain.instance.deadPlayers;
		var ownerName = SteamFriends.GetPersonaName();

		if (playerName != ownerName) //if its not myself, player ded so disable his mesh
		{
			OnlinePlayerObject.instance.renderer.enabled = false;
		}

		if (isAlive)
		{
			if (deadPlayerList.Contains(playerName))
			{
				deadPlayerList.Remove(playerName);
			}
		}
		else
		{
			if (!deadPlayerList.Contains(playerName))
			{
				deadPlayerList.Add(playerName);
			}
		}
	}

	public static void HandleQuestInteractable(Packet _packet)
	{
		string name = _packet.ReadString();
		string text = _packet.ReadString();
		GameObject gameObject = GameObject.Find(name);
		if (gameObject == null) //gObj is probably in X_scene (used for level specific quests)
		{
			gameObject = FindObjectInScene($"{GetLevelId()}", name);
		}
		//bool objFound = gameObject != null;

	/*	if (name == "Q1")
			objFound = true;*/

		if (gameObject != null)
		{
			QuestInteractable questInteractable = gameObject.GetComponent<QuestInteractable>();
			//	QuestOrder questOrder = gameObject.GetComponent<QuestOrder>();
			isNetworkPacket = true;
			switch (text)
			{
				case "AutoComplete":
					if (questInteractable != null)
						questInteractable.AutoComplete();
					break;
				case "CompleteQuest":
					if (questInteractable != null)
						questInteractable.CompleteQuest();
					break;
				case "OnAlternativeQuestComplete":
					if (questInteractable != null)
						questInteractable.OnAlternativeQuestComplete();
					break;
				case "StartQuestFalseFalse":
					if (questInteractable != null)
						questInteractable.StartQuest(false, false);
					break;
				case "StartQuestFalseTrue":
					if (questInteractable != null)
						questInteractable.StartQuest(false, true);
					break;
				case "StartQuestTrueFalse":
					if (questInteractable != null)
						questInteractable.StartQuest(true, false);
					break;
				case "StartQuestTrueTrue":
					if (questInteractable != null)
						questInteractable.StartQuest(true, true);
					break;
				case "StartQuest00":
					if (questInteractable != null)
						questInteractable.StartQuest();
					break;
				case "StartQuest01":
					if (questInteractable != null)
						questInteractable.StartQuest(fromAutoComplete: false, fromCheckpoint: true);
					break;
				case "StartQuest10":
					if (questInteractable != null)
						questInteractable.StartQuest(fromAutoComplete: true);
					break;
				case "StartQuest11":
					if (questInteractable != null)
						questInteractable.StartQuest(fromAutoComplete: true, fromCheckpoint: true);
					break;
				case "StartQuesting":
					RM.questOrder.StartQuesting();
					Debug.Log($"StartQuesting called! -MP");
					break;
				case "CompleteCurrentQuest":
					if (name == "mamaEvil" && GD.currentLevel == LevelEnum._2_MinorMischief)
					{ //this is called on owner side
						AudioController.Play("mama_run", gameObject.transform.position, null);
						AudioController.StopMusic();
					}
					RM.questOrder.CompleteCurrentQuest(gameObject);
					//RM.flashlight.UnfreezeCharge();
					Debug.Log($"Quest Order: {name} " +
				$"called {text}! -MP");
					break;
			}
			if (questInteractable != null)
			{
				Debug.Log($"Quest Interactable: {name} " +
				$"called {text}! -MP");
			}
			isNetworkPacket = false;
		}
		else
		{
			Debug.LogError($"Quest Interactable: {name} was not found\n" +
				$"Tried to call {text} -MP");
		}
	}

	public static GameObject FindObjectInScene(string sceneName, string objectName)
	{
		Scene scene = SceneManager.GetSceneByName(sceneName);
		if (!scene.isLoaded) return null;

		GameObject[] roots = scene.GetRootGameObjects();

		foreach (GameObject root in roots)
		{
			if (root.name == objectName) return root;

			Transform result = root.transform.GetComponentsInChildren<Transform>(true)
				.FirstOrDefault(t => t.name == objectName);

			if (result != null) return result.gameObject;
		}

		return null;
	}
}
