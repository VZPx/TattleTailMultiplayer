using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

[HarmonyPatch(typeof(Trigger))]
public class TriggerPatch
{
	[HarmonyPatch("OnTriggerEnter")]
	[HarmonyPrefix]
	static bool OnTriggerEnterPrefix(Trigger __instance, Collider other)
	{
		var trv = Traverse.Create(__instance);
		bool isInteractable = trv.Field("_isInteractable").GetValue<bool>();

		if (isInteractable)
		{
			return false;
		}
		bool canRun = trv.Method("CanRun").GetValue<bool>();
		if (!canRun)
		{
			return false;
		}
		if (RM.fpsController == null)
		{
			return false;
		}
		if (other.transform.root.gameObject != RM.fpsController.gameObject)
		{
			return false;
		}
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendTrigger(__instance.gameObject.name, false, false);
		}
		var allActions = trv.Field("allActions").GetValue<List<TriggerActionBase>>();

		for (int i = 0; i < allActions.Count; i++)
		{
			allActions[i].RunAction(false);
		}
		__instance.onTriggerEvent.Invoke();
		if (__instance.destroyOnTrigger)
		{
			Object.Destroy(__instance.gameObject);
		}
		return false;
	}
}

public static class TriggerExtensions
{
	public static void CallNormalTriggerMP(this Trigger trigger)
	{
		var trv = Traverse.Create(trigger);
		var allActions = trv.Field("allActions").GetValue<List<TriggerActionBase>>();

		for (int i = 0; i < allActions.Count; i++)
		{
			allActions[i].RunAction(false);
		}

		trigger.onTriggerEvent.Invoke();

		if (trigger.destroyOnTrigger)
		{
			Object.Destroy(trigger.gameObject);
		}
	}
}