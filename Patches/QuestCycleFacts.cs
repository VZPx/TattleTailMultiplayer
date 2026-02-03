using HarmonyLib;
using UnityEngine;
using System.Collections;

[HarmonyPatch(typeof(QuestCycleFacts), "CompleteRoutine")]
public class QuestCycleFactsPatch
{
	[HarmonyPrefix]
	static bool Prefix(QuestCycleFacts __instance, ref IEnumerator __result)
	{
		// Replace the game's Coroutine with our own version
		__result = PatchedCompleteRoutine(__instance);
		return false; // Skip the original
	}

	static IEnumerator PatchedCompleteRoutine(QuestCycleFacts __instance)
	{
		yield return new WaitForSeconds(0.15f);
		AudioController.Play("vhs_glitch", __instance.GetComponentInChildren<TattletailAnimator>().transform.position, null);
		__instance.GetComponentInChildren<TattletailAnimator>().PlayMacro(TattletailAnimator.Macro.DeadBattery, null);
		yield return new WaitForSeconds(__instance.delayBeforeKnock);
		__instance.endTrigger.ManualTrigger(false);
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendTrigger(__instance.endTrigger.name, true, false);
		}
		yield return new WaitForSeconds(__instance.delayBeforeQuestEnd);
		RM.questOrder.CompleteCurrentQuest(__instance.gameObject);
		yield break;
	}
}