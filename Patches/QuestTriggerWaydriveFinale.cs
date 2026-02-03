using HarmonyLib;
using UnityEngine;
using System.Collections;

[HarmonyPatch(typeof(QuestTriggerWaydriveFinale))]
public class WaydriveFinalePatch
{
	// We patch the method that returns the IEnumerator
	[HarmonyPatch("FinalPunchline")]
	[HarmonyPrefix]
	static bool FinalPunchlinePrefix(QuestTriggerWaydriveFinale __instance, ref IEnumerator __result)
	{
		// Replace with our custom networked version
		__result = PatchedPunchline(__instance);
		return false; // Skip the original
	}

	static IEnumerator PatchedPunchline(QuestTriggerWaydriveFinale __instance)
	{
		__instance.jumpscareHolder.SetActive(false);
		Camera componentInChildren = RM.fpsController.GetComponentInChildren<Camera>();
		RM.tattletail.SetTattletail(false, false);
		CameraClearFlags clearFlags = componentInChildren.clearFlags;
		Color backgroundColor = componentInChildren.backgroundColor;
		int cullingMask = componentInChildren.cullingMask;
		RM.fpsController.TeleportPlayer(__instance.darkRoomSpawnpoint);
		AudioController.Play("flashlight_die", RM.fpsController.transform);
		RM.hud.questText.text = "???";
		RM.hud.questText.Rebuild(float.PositiveInfinity);
		yield return new WaitForSeconds(__instance.darknessIdleTime);
		AudioController.Play("k_mama_punchline", RM.fpsController.transform);
		yield return new WaitForSeconds(__instance.jumpscareAnimationDelayTime);
		__instance.jumpscareHolder.SetActive(true);
		__instance.jumpscareAnimator.SetTrigger(__instance.jumpscareTriggerName);
		yield return new WaitForSeconds(__instance.delayBeforeNextLevel);
		GameManager.PlayLevel(LevelEnum._Memory_10_EscapeTheKaleidoscope);
		if (!HandleData.isNetworkPacket)
		{
			SendData.SendLevelTransition(LevelEnum._Memory_10_EscapeTheKaleidoscope.ToString());
		}
		yield break;
	}
}