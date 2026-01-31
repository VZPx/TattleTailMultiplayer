using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(SceneLoader))]
public class SceneLoaderPatch
{
	[HarmonyPatch("LoadScene", new Type[] { typeof(List<string>), typeof(Action) })]
	[HarmonyPrefix]
	static bool LoadScenePrefix(SceneLoader __instance, List<string> scenes,
		Action onComplete)
	{
		var trv = Traverse.Create(__instance);
		var loadRoutine = trv.Field("_loadRoutine");
		var routineEnumerator = LoadRoutinePatch(__instance, scenes, onComplete);
		loadRoutine.SetValue(__instance.StartCoroutine(routineEnumerator));
		return false;
	}

	static IEnumerator LoadRoutinePatch(SceneLoader __instance,
		List<string> scenes, Action onComplete)
	{
		var trv = Traverse.Create(__instance);

		var isLoading = trv.Field("_isLoading");
		var loadRoutine= trv.Field("_loadRoutine");
	
		//__instance._isLoading = true;
		isLoading.SetValue(true);
		if (SingletonMonoBehaviour<AudioController>.DoesInstanceExist())
		{
			AudioController.MuteSound(true);
		}
		int num;
		for (int i = 0; i < scenes.Count; i = num + 1)
		{
			AsyncOperation operation = new AsyncOperation();
			if (i == 0)
			{
				operation = SceneManager.LoadSceneAsync(scenes[i], 0);
			}
			else
			{
				operation = SceneManager.LoadSceneAsync(scenes[i], (LoadSceneMode)1);
			}
			operation.allowSceneActivation = false;
			if (operation == null)
			{
				//__instance.StopCoroutine(__instance._loadRoutine);
				//__instance._loadRoutine = null;
				__instance.StopCoroutine(loadRoutine.GetValue<Coroutine>());
				loadRoutine.SetValue(null);
				Debug.Log("Operation == null -SceneLoader");
				yield return null;
			}
			if (i == 0)
			{
				GameConsole.CloseConsole();
				if (RM.fpsController != null)
				{
					RM.fpsController.SetPlayerActive(false);
				}
				__instance.graphicsHolder.SetActive(true);
				__instance.loadingText.Rebuild();
			}
			while (operation.progress < 0.9f)
			{
				Debug.Log("operation.progress < 0.9f");
				yield return 0;
			}
			operation.allowSceneActivation = true;
			while (!operation.isDone)
			{
				Debug.Log("operation is not done!");
				yield return 0;
			}
			operation = null;
			num = i;
			operation = null;
			operation = null;
		}
		var _vhsFadeTime = trv.Field("vhsFadeTime").GetValue<float>();
		var _minLoadTime = trv.Field("minLoadTime").GetValue<float>();
		float timer = _vhsFadeTime;
		while (timer > 0f)
		{
			float num2 = 1f - timer / _vhsFadeTime;
			__instance.vhs.gammaCorection = AxKEasing.EaseOutExpo(3f, 1.25f, num2);
			timer -= 1f * Time.deltaTime;
			yield return 0;
		}
		timer = _minLoadTime;
		while (timer > 0f)
		{
			timer -= 1f * Time.deltaTime;
			yield return 0;
		}
		timer = _vhsFadeTime;
		while (timer > 0f)
		{
			float num3 = timer / _vhsFadeTime;
			__instance.vhs.gammaCorection = AxKEasing.EaseOutExpo(3f, 1.25f, num3);
			timer -= 1f * Time.deltaTime;
			yield return 0;
		}
		Action action = onComplete;
		onComplete = null;
		if (action != null)
		{
			action.Invoke();
		}
		__instance.graphicsHolder.SetActive(false);
		//__instance._loadRoutine = null;
		loadRoutine.SetValue(null);
		AudioController.MuteSound(false);
		AudioController.SetGlobalVolume(0f);
		float inc = 0.02f;
		while (AudioController.GetGlobalVolume() < 1f)
		{
			AudioController.SetGlobalVolume(AudioController.GetGlobalVolume() + inc);
			yield return 0;
		}
		AudioController.SetGlobalVolume(1f);
		//__instance._isLoading = false;
		isLoading.SetValue(false);
		yield break;
	}
}