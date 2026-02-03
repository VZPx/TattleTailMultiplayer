using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(QuestMamaLOS))]
public class QuestMamaLOSPatch
{
	[HarmonyPatch("CheckForCompletion")]
	[HarmonyPrefix]
	static bool CheckForCompletionPrefix(QuestMamaLOS __instance)
	{
		var trv = Traverse.Create(__instance);
		var waitForCompletion = trv.Method("WaitForCompletion").GetValue<IEnumerator>();

		if (RM.fpsController == null)
		{
			return false;
		}
		if (__instance.hasSeenMama)
		{
			if (LOSUtil.CanPlayerSeePos(__instance.transform.position, PatrolNode.GetVisObjOff(), false))
			{
				RM.flashlight.ZeroOutLightTimer();
			}
			return false;
		}
		if (LOSUtil.CanPlayerSeePos(__instance.transform.position, PatrolNode.GetVisObjOff(), false))
		{
			__instance.lookTime -= 1f * Time.deltaTime;
			if (__instance.lookTime <= 0f)
			{
				__instance.lookTime = float.PositiveInfinity;
				__instance.hasSeenMama = true;
				RM.flashlight.KillLight(true);
				RM.flashlight.FreezeCharge();
				__instance.StartCoroutine(waitForCompletion);
			}
		}
		if (Vector3.Distance(RM.fpsController.transform.position, __instance.transform.position) < __instance.forceCompletionDistance)
		{
			__instance.lookTime = float.PositiveInfinity;
			__instance.hasSeenMama = true;
			RM.flashlight.KillLight(true);
			RM.flashlight.FreezeCharge();
			AudioController.Play("mama_run", __instance.gameObject.transform.position, null);
			RM.questOrder.CompleteCurrentQuest(__instance.gameObject);
			if (!HandleData.isNetworkPacket)
			{
				SendData.SendQuestInteractable(__instance.gameObject.name, "CompleteCurrentQuest");
			}
		}
		return false;
	}

	[HarmonyPatch("WaitForCompletion")]
	[HarmonyPrefix]
	static bool WaitForCompletionPrefix(QuestMamaLOS __instance, ref IEnumerator __result)
	{
		__result = WaitForCompletionPatch(__instance);
		return false;
	}

	static IEnumerator WaitForCompletionPatch(QuestMamaLOS __instance)
	{
		yield return new WaitForSeconds((float)__instance.waitTime);
		AudioController.Play("mama_run", __instance.gameObject.transform.position, null);
		RM.questOrder.CompleteCurrentQuest(__instance.gameObject);
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendQuestInteractable(__instance.gameObject.name, "CompleteCurrentQuest");
		}
		RM.flashlight.UnfreezeCharge();
		if (__instance.stopMusic)
		{
			AudioController.StopMusic();
		}
		yield break;
	}
}