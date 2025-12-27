using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(QuestBase), "Trigger")]
public class QuestBasePatch
{
	[HarmonyPrefix]
	static bool Prefix(QuestBase __instance)
	{
		if (!(__instance.GetComponent<Interactable>() != null))
		{
			Trigger component = __instance.GetComponent<Trigger>();
			if (component != null && component.GetIsInteractable())
			{
				component.ManualTrigger(false);
				SendData.SendTrigger(component.name, true, false);
			}
		}
		return false;
	}
}