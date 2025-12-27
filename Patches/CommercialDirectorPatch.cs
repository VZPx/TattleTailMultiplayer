using BepInEx;
using UnityEngine;
using HarmonyLib;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[HarmonyPatch(typeof(CommercialDirectorPlayable), "Setup")]
public class CommercialDirectorPatch
{
	[HarmonyPrefix]
	static bool Prefix(CommercialDirectorPlayable __instance, ref IEnumerator __result)
	{
		// We set the __result to OUR custom version of the enumerator
		__result = PatchedSetup(__instance);

		// Return false so the original Setup() NEVER runs
		return false;
	}

	static IEnumerator PatchedSetup(CommercialDirectorPlayable __instance)
	{
		var trv = Traverse.Create(__instance);
		float[] lightsStartIntensity = new float[__instance.lights.Length];
		//__instance.lightsStartIntensity = new float[__instance.lights.Length];
		for (int i = 0; i < __instance.lights.Length; i++)
		{
			//this.lightsStartIntensity[i] = this.lights[i].intensity;
			lightsStartIntensity[i] = __instance.lights[i].intensity;
		}
		trv.Field("lightsStartIntensity").SetValue(lightsStartIntensity);
		__instance.SetLights(false);
		__instance.anim.enabled = false;
		Object.Destroy(__instance.anim);
		yield return 0;
		__instance.anim = __instance.gameObject.AddComponent<Animation>();
		yield return 0;
		__instance.anim.playAutomatically = false;
		__instance.anim.animatePhysics = false;
		__instance.playButton.transform.SetParent(__instance.transform);
		Button[] shotButtons = new Button[__instance.shots.Length];
		//__instance.shotButtons = new Button[__instance.shots.Length];
		for (int j = 0; j < __instance.shots.Length; j++)
		{
			__instance.anim.AddClip(__instance.shots[j].clip, __instance.shots[j].shotName);
			if (!__instance.automatedPlay || __instance.editorRecordAutomatedShots)
			{
				if (!__instance.shots[j].secret)
				{
					GameObject gameObject = Utils.InstantiateUI(__instance.shotButtonTemplate.gameObject, "Button " + __instance.shots[j].shotName, __instance.shotButtonTemplate.transform.parent);
					shotButtons[j] = gameObject.GetComponent<Button>();
					shotButtons[j].GetComponentInChildren<Text>().text = string.Concat(new object[]
					{
						"CAM",
						j,
						": ",
						__instance.shots[j].shotName
					});
					string shotName = __instance.shots[j].shotName;
					shotButtons[j].onClick.AddListener(delegate
					{
						__instance.TryPlayShot(shotName);
						SendData.SendCommercialAction(shotName);
					});
					GameObject.Find("Play Button").GetComponent<Button>().onClick.AddListener(delegate
					{
						SendData.SendCommercialAction("");
					});
					__instance.shots[j].camButton = shotButtons[j];
				}
			}
			if (__instance.shots[j].activeBefore != null)
			{
				__instance.shots[j].activeBefore.SetActive(true);
			}
			if (__instance.shots[j].activeAfter != null)
			{
				__instance.shots[j].activeAfter.SetActive(false);
			}
		}
		trv.Field("shotButtons").SetValue(shotButtons);
		__instance.playButton.transform.SetParent(__instance.shotButtonTemplate.transform.parent);
		__instance.shotButtonTemplate.gameObject.SetActive(false);
		trv.Method("UpdateMode").GetValue();
		//__instance.UpdateMode();
		AudioController.Play("vhs_noise_loop", __instance.handheldCamera.transform);
		AudioController.MuteSound(false);
		yield break;
	}
}