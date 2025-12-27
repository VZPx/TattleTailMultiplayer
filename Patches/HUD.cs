using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;


[HarmonyPatch(typeof(HUD))]
public class HUDPatch
{
	public static bool hasInputted;

	[HarmonyPatch("ItemRoutine")]
	[HarmonyPrefix]
	static bool ItemPrefix(HUD __instance, HUDItem item, ref IEnumerator __result)
	{
		// Replace the original routine with our patched version
		__result = PatchedItemRoutine(__instance, item);
		return false; // Skip original
	}

	static IEnumerator PatchedItemRoutine(HUD __instance, HUDItem item)
	{
		var trv = Traverse.Create(__instance);
		RM.playerActionController.SetCanInteract(false);
		if (__instance.eggCollectClip)
		{
			AudioSource.PlayClipAtPoint(__instance.eggCollectClip, RM.fpsController.transform.position);
		}
		TimeManager.SetTimeScale(0f);
		GS.acceptInput = false;
		__instance.itemGroup.gameObject.SetActive(true);
		__instance.itemDisplayImage.CrossFadeAlpha(1f, 0f, true);
		__instance.itemDisplayImage.transform.localScale = Vector3.one;
		__instance.itemDisplayImage.gameObject.SetActive(false);
		__instance.itemNameText.text = string.Empty;
		__instance.itemNameText.Rebuild();
		__instance.itemDescriptionText.text = string.Empty;
		__instance.itemDescriptionText.Rebuild();
		__instance.itemTextText.text = string.Empty;
		__instance.itemTextText.Rebuild();
		trv.Field("itemTextCompleted").SetValue(false);
		//__instance.itemTextCompleted = false;
		float timer = 0.15f;
		while (timer > 0f)
		{
			__instance.itemGroup.alpha = 1f - timer / 0.15f;
			timer -= 1f * Time.unscaledDeltaTime;
			yield return 0;
		}
		__instance.itemGroup.alpha = 1f;

		hasInputted = false;
		//trv.Field("hasInputted").SetValue(false);
		//__instance.hasInputted = false;
		if (item.HasImage())
		{
			__instance.itemDisplayImage.sprite = item.itemImage;
			__instance.itemDisplayImage.gameObject.SetActive(true);
			__instance.itemDisplayImage.transform.EaseLocalScaleFrom(new Vector3(0f, 2f, 1f), 0.5f, AxKEasing.EaseType.EaseOutElastic, null, true);
			yield return new WaitForSecondsRealtime(0.87f);
		}
		__instance.itemNameText.text = __instance.eggContentPrefix + item.itemName;
		__instance.itemNameText.Rebuild();
		yield return new WaitForSecondsRealtime(1.5f);
		if (item.itemDescription != string.Empty)
		{
			__instance.itemDescriptionText.text = item.itemDescription;
			__instance.itemDescriptionText.Rebuild();
		}
		if (item.itemText != string.Empty)
		{
			__instance.itemTextText.text = item.itemText;
			__instance.itemTextText.Rebuild();
			if (item.HasImage())
			{
				__instance.itemDisplayImage.CrossFadeAlpha(0.065f, 0.4f, true);
			}
			while (!trv.Field("itemTextCompleted").GetValue<bool>()) //itemTextCompleted
			{
				if (Input.GetButtonDown("Interact"))
				{
					__instance.itemTextText.SpeedRead();
				}
				yield return 0;
			}
		}
		while (!hasInputted)
		{
			if (Input.GetButtonDown("Interact"))
			{
				hasInputted = true;
				SendData.SendInput(true);
			}
			yield return 0;
		}
		timer = 0.15f;
		AudioController.Play("music_sting_ending", RM.fpsController.transform.position, null);
		while (timer > 0f)
		{
			__instance.itemGroup.alpha = timer / 0.15f;
			timer -= 1f * Time.unscaledDeltaTime;
			yield return 0;
		}
		__instance.itemGroup.alpha = 0f;
		__instance.itemDescriptionText.text = string.Empty;
		__instance.itemDescriptionText.Rebuild();
		__instance.itemNameText.text = string.Empty;
		__instance.itemNameText.Rebuild();
		__instance.itemTextText.text = string.Empty;
		__instance.itemTextText.Rebuild();
		__instance.itemGroup.gameObject.SetActive(false);
		TimeManager.SetTimeScale(1f);
		GS.acceptInput = true;
		item.OnCollect();
		GD.OnCollectItem(item.itemType);
		RM.playerActionController.SetCanInteract(true);
		if (item.waydriveEnding)
		{
			QuestWaydriveEnding questWaydriveEnding = UnityEngine.Object.FindObjectOfType<QuestWaydriveEnding>();
			if (questWaydriveEnding != null)
			{
				questWaydriveEnding.OnOpenedGifts();
			}
		}
		hasInputted = false;
		//__instance.hasInputted = false;
		yield break;
	}

	[HarmonyPatch("TitleRoutine")] // Assuming HUDController
	[HarmonyPrefix]
	static bool TitlePrefix(HUD __instance, LevelEnum level, bool alternateTheme, ref IEnumerator __result)
	{
		// Replace with our custom routine
		__result = PatchedTitleRoutine(__instance, level, alternateTheme);
		return false; // Don't run the original
	}

	static IEnumerator PatchedTitleRoutine(HUD __instance, LevelEnum level, bool alternateTheme)
	{
		var trv = Traverse.Create(__instance);

		Time.timeScale = 0f;
		GS.acceptInput = false;
		__instance.titleHolder.SetActive(true);
		CanvasGroup titleGroup = __instance.titleHolder.GetComponentInChildren<CanvasGroup>();
		titleGroup.alpha = 1f;
		__instance.titleText.text = string.Empty;
		__instance.titleText.Rebuild();
		Cursor.lockState = (CursorLockMode)1;
		yield return new WaitForSecondsRealtime(1.5f);
		AudioController.MuteSound(false);
		yield return 0;
		AudioObject obj;
		if (alternateTheme)
		{
			obj = AudioController.PlayMusic("music_theme_flipped", 1f, 0f, 0f);
		}
		else
		{
			obj = AudioController.PlayMusic("music_theme", 1f, 0f, 0f);
		}
		obj.GetComponent<AudioSource>().volume = 1f;

		LevelData levelByEnum = Utils.GetLevelFlow().GetLevelByEnum(level);
		string text = string.Concat(new string[] { __instance.titleTextLineOnePrefix, 
			levelByEnum.introLineOne, __instance.titleTextLineOnePostFix, 
			"\n", __instance.titleTextLineTwoPrefix, 
			levelByEnum.introLineTwo, __instance.titleTextLineTwoPostfix });
		__instance.titleText.text = text;


		__instance.titleText.Rebuild();

		yield return new WaitForSecondsRealtime(0.7f);
		float timer = 6f;


		hasInputted = false;

		//__instance.hasInputted = false;

		while (!hasInputted)
		{
			if (timer <= 0f)
			{
				hasInputted = true;
			}
			else if (Input.GetButtonDown("Interact"))
			{
				hasInputted = true;
				SendData.SendInput(true);
			}
			timer -= 1f * Time.unscaledDeltaTime;
			yield return 0;
		}

		float fadeTimer = 1.2f;
		AudioController.StopMusic(1.2f);
		Time.timeScale = 1f;
		GS.acceptInput = true;
		yield return 0;
		obj = AudioController.Play("music_sting_ending", RM.fpsController.transform, 1f, 1f, 0f);
		while (fadeTimer > 0f)
		{
			obj.GetComponent<AudioSource>().volume = 1f;
			Vector3 newScale = new Vector3(1f, AxKEasing.EaseOutQuad(0f, 1f, fadeTimer / 1.2f), 1f);
			__instance.eyeTop.localScale = newScale;
			__instance.eyeBottom.localScale = newScale;
			titleGroup.alpha = fadeTimer / 1.2f;
			fadeTimer -= 1f * Time.deltaTime;
			yield return 0;
		}
		titleGroup.alpha = 0f;
		__instance.titleHolder.SetActive(false);
		hasInputted = false;
		yield break;
	}
}