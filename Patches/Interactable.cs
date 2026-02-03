using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using HighlightingSystem;


[HarmonyPatch(typeof(Interactable), "OnHold")]
public class InteractablePatch
{
	[HarmonyPrefix]
	static bool Prefix(Interactable __instance)
	{
		var trv = Traverse.Create(__instance);
		bool isHeld = trv.Field("_isHeld").GetValue<bool>();

		if (isHeld)
		{
			trv.Field("_holdEndTimer").SetValue(0.05f);
			//__instance._holdEndTimer = 0.05f;


			float holdTimer = trv.Field("_holdTimer").GetValue<float>();
			holdTimer += 1f * Time.deltaTime;
			trv.Field("_holdTimer").SetValue(holdTimer);
			//__instance._holdTimer += 1f * Time.deltaTime;

			float holdTime = trv.Field("holdTime").GetValue<float>();
			if (holdTimer >= holdTime)
			{
				if (trv.Field("holdAudio").GetValue<AudioObject>() != null)
				{
					AudioController.Stop("hold_loop");
					trv.Field("holdAudio").SetValue(null);
					//this.holdAudio = null;
				}
				if (trv.Field("holdSpecificAudio").GetValue<AudioObject>() != null)
				{
					AudioController.Stop(__instance.holdAudioName);
					trv.Field("holdSpecificAudio").SetValue(null);
					//this.holdSpecificAudio = null;
				}
				trv.Field("_isHeld").SetValue(false);
				//this._isHeld = false;
				GS.acceptInput = true;
				__instance.OnInteractComplete(false);
				AudioController.Play("hold_start", RM.fpsController.transform);
				string objectName = (__instance.transform.parent ? __instance.gameObject.name 
					: __instance.transform.parent.name);
				SendData.SendInteraction(Camera.main.transform.position, 
					Camera.main.transform.forward, false, objectName);
			}
		}

		return false;
	}

}

[HarmonyPatch(typeof(Interactable), "OnInteract")]
public class OnInteractPatch
{
	private static Color COLOR_HOVER_ENABLED = Color.green;
	private static Color COLOR_HOVER_DISABLED = Color.black;
	private static Color COLOR_HOLD = Color.yellow;

	[HarmonyPrefix]
	static bool Prefix(Interactable __instance)
	{
		var trv = Traverse.Create(__instance);

		if (__instance.holdTime <= 0f || GS.noHoldTime)
		{
			__instance.OnInteractComplete(false);
			string objectName = (__instance.transform.parent ? __instance.gameObject.name 
				: __instance.transform.parent.name);
			SendData.SendInteraction(Camera.main.transform.position, Camera.main.transform.forward, false, objectName);
			return false;
		}
		trv.Field("_isHeld").SetValue(true);
		//__instance._isHeld = true;
		trv.Field("_holdEndTimer").SetValue(0.05f);
		//__instance._holdEndTimer = 0.05f;
		trv.Field("_holdTimer").SetValue(0f);
		//__instance._holdTimer = 0f;
		if (__instance.highlight)
		{
			trv.Field("_highlighter").GetValue<Highlighter>()
				.FlashingOn(COLOR_HOLD, COLOR_HOVER_DISABLED);
			//__instance._highlighter.FlashingOn(COLOR_HOLD, COLOR_HOVER_DISABLED);
		}
		GS.acceptInput = false;
		trv.Field("holdAudio").SetValue
			(AudioController.Play("hold_loop", RM.fpsController.transform));
		//__instance.holdAudio = AudioController.Play("hold_loop", RM.fpsController.transform);
		if (__instance.holdAudioName != string.Empty)
		{
			trv.Field("holdSpecificAudio").SetValue
			(AudioController.Play
				(__instance.holdAudioName, RM.fpsController.transform));
			/*__instance.holdSpecificAudio = AudioController.Play
				(__instance.holdAudioName, RM.fpsController.transform);*/
		}

		return false;
	}
}

[HarmonyPatch(typeof(Interactable), "OnInteractComplete")]
public class OnInteractCompletePatch
{
	[HarmonyPrefix]
	static bool Prefix(Interactable __instance, bool fromAutoComplete)
	{
		var trv = Traverse.Create(__instance);

		var highlighter = trv.Field("_highlighter").GetValue<Highlighter>();
		var honker = trv.Field("honker").GetValue<Honker>();

		if (highlighter != null)
		{
			highlighter.FlashingOff();
		}
		__instance.onInteractEvent.Invoke();
		if (__instance.onInteractClip && !fromAutoComplete)
		{
			AudioSource.PlayClipAtPoint(__instance.onInteractClip, __instance.transform.position);
		}
		if (__instance.storeColorOnInteract)
		{
			RM.recolorManager.storedColor = __instance.colorToStore;
		}
		__instance.actions.PlayActions(__instance, fromAutoComplete);
		if (__instance.hint != string.Empty && !fromAutoComplete)
		{
			RM.hud.SetNewHint(__instance.hint, 4f);
		}
		if (__instance.eggToCollect != 0)
		{
			RM.hud.OnCollectEgg(__instance.eggToCollect);
		}
		if (__instance.honks && honker != null && !fromAutoComplete)
		{
			honker.OnMouseDown();
			//__instance.honker.OnMouseDown();
		}
		if (__instance.setObjectActiveOnInteract)
		{
			__instance.setObjectActiveOnInteract.SetActive(true);
		}
		if (__instance.setObjectInactiveOnInteract)
		{
			__instance.setObjectInactiveOnInteract.SetActive(false);
		}
		for (int i = 0; i < __instance.additionalActiveObjects.Count; i++)
		{
			if (__instance.additionalActiveObjects[i] != null)
			{
				__instance.additionalActiveObjects[i].SetActive(true);
			}
		}
		for (int j = 0; j < __instance.additionalInactiveObjects.Count; j++)
		{
			if (__instance.additionalInactiveObjects[j] != null)
			{
				__instance.additionalInactiveObjects[j].SetActive(false);
			}
		}
		if (__instance.updateHUDText)
		{
			RM.hud.SetNewQuest(__instance.newHUDQuest);
		}
		if (__instance.recolorObjectOnInteract != null)
		{
			RM.recolorManager.RecolorObject(__instance.recolorObjectOnInteract, __instance.newColor);
		}
		if (__instance.itemToCollect != null)
		{
			RM.hud.OnCollectItem(__instance.itemToCollect);
		}
		Trigger component = __instance.GetComponent<Trigger>();
		if (component != null)
		{
			component.ManualTrigger(fromAutoComplete);
			SendData.SendTrigger(component.gameObject.name, true, fromAutoComplete);
		}
		if (__instance.newStateOnInteract != __instance.currentState)
		{
			__instance.currentState = __instance.newStateOnInteract;
		}
		if (__instance.setInactiveOnInteract)
		{
			__instance.gameObject.SetActive(false);
		}

		var collider = trv.Field("_collider").GetValue<Collider>();

		if (collider != null)
		{
			if (__instance.currentState == Interactable.State.Hidden)
			{
				collider.enabled = false;
				return false;
			}
			collider.enabled = true;
		}

		return false;
	}
}

[HarmonyPatch(typeof(Interactable), "Start")]
public class InteractableInjectionPatch
{
	static void Postfix(Interactable __instance)
	{
		// Add our watchdog if it's not already there
		if (__instance.gameObject.GetComponent<InteractableWatchdog>() == null)
		{
			var watchdog = __instance.gameObject.AddComponent<InteractableWatchdog>();
			watchdog.target = __instance;
		}
	}
}