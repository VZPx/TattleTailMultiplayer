using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(QuestSeek), "AutoComplete")]
public class QuestSeekPatch
{
	[HarmonyPrefix]
	static bool AutoCompletePrefix(QuestSeek __instance)
	{
		__instance.gameObject.SetActive(true);
		__instance.gameObject.GetComponent<Interactable>().InitializeTask();
		__instance.gameObject.GetComponent<Interactable>().currentState = Interactable.State.Enabled;
		if (__instance.activateOnStart != null)
		{
			__instance.activateOnStart.SetActive(true);
		}
		__instance.gameObject.GetComponent<Interactable>().OnInteractComplete(true);
		RM.questOrder.CompleteCurrentQuest(__instance.gameObject);
		GameObject gameObject = GameObject.Find("ConvoStopTrigger");
		if (gameObject != null)
		{
			gameObject.GetComponent<Trigger>().ManualTrigger(true);
			if (!HandleData.isNetworkPacket)
			{
				SendData.SendTrigger(gameObject.name, true, true);
			}
		}

		return false;
	}
}