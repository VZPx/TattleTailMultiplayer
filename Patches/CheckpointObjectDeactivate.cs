using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;


[HarmonyPatch(typeof(CheckpointObjectDeactivate), "OnCheckpoint")] 
public class CheckpointObjectivePatch
{
	[HarmonyPostfix]
	static void Postfix(CheckpointObjectDeactivate __instance)
	{
		for (int k = 0; k < __instance.triggerObjects.Count; k++)
		{
			Trigger component = __instance.triggerObjects[k].GetComponent<Trigger>();
			if (component != null)
			{
				if (!HandleData.isNetworkPacket)
				{
					SendData.SendTrigger(component.name, true, true);
				}
			}
		}
	}
}