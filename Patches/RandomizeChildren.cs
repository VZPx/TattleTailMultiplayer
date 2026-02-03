using HarmonyLib;
using UnityEngine;
using Steamworks;

[HarmonyPatch(typeof(RandomizeChildren))]
public class RandomizeChildrenPatch
{
	[HarmonyPatch("Awake")]
	[HarmonyPrefix]
	static bool AwakePrefix(RandomizeChildren __instance)
	{
		if (__instance.cheatMode)
		{
			bool isEditor = Application.isEditor;
			return false;
		}
		if (SteamMatchmaking.GetLobbyOwner(SteamLobby.lobby) == SteamUser.GetSteamID())
		{
			Traverse.Create(__instance).Method("Swap").GetValue();
		}
		return false;
	}

	[HarmonyPatch("Swap")]
	[HarmonyPrefix]
	static bool SwapPrefix(RandomizeChildren __instance)
	{
		Transform trans = __instance.transform;
		int count = trans.childCount;

		for (int i = 0; i < count; i++)
		{
			// Pick a random child index
			int num = Mathf.RoundToInt((float)Random.Range(0, count));

			Transform childA = trans.GetChild(num);
			Transform childB = trans.GetChild(i);

			// Store original A data
			Vector3 posA = childA.position;
			Quaternion rotA = childA.rotation;

			// 1. Physically swap them in the host's world
			childA.position = childB.position;
			childA.rotation = childB.rotation;

			childB.position = posA;
			childB.rotation = rotA;

			// 2. SEND THE DATA TO CLIENTS
			// Send new data for Child A (which now has B's old spot)
			if (!HandleData.isNetworkPacket)
			{
				SendData.SendSwap(childA.name, childB.position, childB.rotation);

				// Send new data for Child B (which now has A's old spot)
				SendData.SendSwap(childB.name, posA, rotA);
			}
		}

		return false; // Skip original
	}
}