using HarmonyLib;
using System;
using UnityEngine;

public class HandleData
{
	public static void WelcomeReceived(Packet _packet)
	{
		Debug.Log(_packet.ReadString());
	}

	public static void MovementReceived(Packet _packet)
	{
		Movement movement = PlayerObject.instance._Movement;
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
		GameManager.PlayLevel(GetEnumValue(_packet.ReadString()));
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

	public static void CommericalAction(Packet _packet)
	{
		string text = _packet.ReadString();
		Debug.Log("Changed camera!");
		if (text == "")
		{
			UnityEngine.Object.FindObjectOfType<CommercialDirectorPlayable>().OnHitPlayButton();
		}
		else
		{
			UnityEngine.Object.FindObjectOfType<CommercialDirectorPlayable>().TryPlayShot(text);
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
		if (gameObject != null)
		{
			Trigger component = gameObject.GetComponent<Trigger>();
			if (component != null)
			{
				if (!flag)
				{
					component.CallNormalTriggerMP();
				}
				else
				{
					component.ManualTrigger(fromAutoComplete);
				}
			}
		}
		else
		{
			Debug.LogError("Trigger Object was not found -MP");
		}
	}

	public static void HandleQuestTrigger(Packet _packet)
	{
		string name = _packet.ReadString();
		bool flag = _packet.ReadBool();
		bool flag2 = _packet.ReadBool();
		GameObject gameObject = GameObject.Find(name);
		if (gameObject != null)
		{
			QuestTrigger component = gameObject.GetComponent<QuestTrigger>();
			if (component != null)
			{
				if (flag || (!flag && !flag2))
				{
					RM.questOrder.CompleteCurrentQuest(gameObject);
				}
				else
				{
					component.CompleteQuest();
				}
				Debug.Log("HandleQuestTrigger() -MP");
			}
		}
		else
		{
			Debug.LogError("Quest Trigger Object was not found -MP");
		}
	}

	public static void HandleQuestInteractable(Packet _packet)
	{
		string name = _packet.ReadString();
		string text = _packet.ReadString();
		GameObject gameObject = GameObject.Find(name);
		if (gameObject != null)
		{
			QuestInteractable component = gameObject.GetComponent<QuestInteractable>();
			if (component != null)
			{
				switch (text)
				{
					case "AutoComplete":
						component.AutoComplete();
						break;
					case "CompleteQuest":
						component.CompleteQuest();
						break;
					case "OnAlternativeQuestComplete":
						component.OnAlternativeQuestComplete();
						break;
					case "StartQuest00":
						component.StartQuest();
						break;
					case "StartQuest01":
						component.StartQuest(fromAutoComplete: false, fromCheckpoint: true);
						break;
					case "StartQuest10":
						component.StartQuest(fromAutoComplete: true);
						break;
					case "StartQuest11":
						component.StartQuest(fromAutoComplete: true, fromCheckpoint: true);
						break;
				}
				Debug.Log(text + " on QuestInteractable -MP");
			}
		}
		else
		{
			Debug.LogError("Quest Trigger Interactable was not found -MP");
		}
	}
}
