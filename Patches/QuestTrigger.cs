using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(QuestTrigger))]
public class QuestTriggerPatch
{
	[HarmonyPatch("OnTriggerEnter")]
	[HarmonyPrefix]
	static bool OnTriggerEnterPrefix(QuestTrigger __instance, Collider other)
	{
		if (RM.fpsController == null)
		{
			return false;
		}
		if (other.transform.root.gameObject == RM.fpsController.gameObject)
		{
			RM.questOrder.CompleteCurrentQuest(__instance.gameObject);
			SendData.SendQuestTrigger(__instance.gameObject.name, false, false);
		}
		return false;
	}

	[HarmonyPatch("AutoComplete")]
	[HarmonyPrefix]
	static bool AutoCompletePrefix(QuestTrigger __instance)
	{
		Trigger component = __instance.GetComponent<Trigger>();
		if (component != null)
		{
			component.ManualTrigger(true);
			SendData.SendTrigger(component.name, true, true);
		}
		RM.questOrder.CompleteCurrentQuest(__instance.gameObject);
		return false;
	}
}