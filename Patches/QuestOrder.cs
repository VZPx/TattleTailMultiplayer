using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;

[HarmonyPatch(typeof(QuestOrder))]
public class QuestOrderPatch
{
	[HarmonyPatch("StartQuesting")]
	[HarmonyPrefix]
	static bool StartQuestingPrefix(QuestOrder __instance)
	{
		var trv = Traverse.Create(__instance);
		bool advanceToCheckpoint = trv.Field("advanceToCheckpoint").GetValue<bool>();
		QuestBase currentQuest = trv.Field("currentQuest").GetValue<QuestBase>();

		if (advanceToCheckpoint)
		{
			if (currentQuest != null)
			{
				currentQuest.AutoComplete();
				SendData.SendQuestInteractable(currentQuest.name, "AutoComplete");
			}

			string needed = trv.Field("neededCheckpoint").GetValue<string>();
			string storedCheckpoint = trv.Field("storedCheckpoint").GetValue<string>();

			while (needed != storedCheckpoint)
			{
				__instance.ActivateNextQuest(true, false);
				currentQuest.AutoComplete();
				SendData.SendQuestInteractable(currentQuest.name, "AutoComplete");
			}
			__instance.ExitCheckpointMode();
			__instance.ActivateNextQuest(false, true);
			return false;
		}
		__instance.ActivateNextQuest(false, false);
		return false;
	}

	[HarmonyPatch("CompleteCurrentQuest")]
	[HarmonyPrefix]
	static bool CompleteCurrentQuestPrefix(QuestOrder __instance, GameObject sender)
	{
		var trv = Traverse.Create(__instance);
		var questList = __instance.questList;
		int currentQuestIndex = trv.Field("currentQuestIndex").GetValue<int>();
		var possibleQuests = trv.Field("possibleQuests").GetValue<List<QuestBase>>();
		var availableQuests = trv.Field("availableQuests").GetValue<Dictionary<QuestType, List<QuestBase>>>();

		bool advanceUponCompletion = __instance.questList[currentQuestIndex].advanceUponCompletion;
		currentQuestIndex++;
		trv.Field("currentQuestIndex").SetValue(currentQuestIndex);
		var currentQuest = trv.Field("currentQuest");

		if (possibleQuests.Count > 0)
		{
			for (int i = 0; i < possibleQuests.Count; i++)
			{
				if (possibleQuests[i].gameObject == sender)
				{
					trv.Field("currentQuest").SetValue(possibleQuests[i]);
					//__instance.currentQuest = possibleQuests[i];
					availableQuests[currentQuest.GetValue<QuestBase>().questType].Remove(currentQuest.GetValue<QuestBase>());
				}
				else
				{
					possibleQuests[i].OnAlternativeQuestComplete();
					SendData.SendQuestInteractable(possibleQuests[i].name, "OnAlternativeQuestComplete");
				}
			}
			possibleQuests.Clear();
		}
		currentQuest.GetValue<QuestBase>().CompleteQuest();
		SendData.SendQuestInteractable(currentQuest.GetValue<QuestBase>().name, "CompleteQuest");
		currentQuest.SetValue(null);
		RM.tattletail.GetNeeds().OnQuestComplete();

		bool advanceToCheckpoint = trv.Field("advanceToCheckpoint").GetValue<bool>();

		if (!advanceToCheckpoint)
		{
			RM.hud.ClearQuest();
		}
		if (currentQuestIndex >= questList.Count)
		{
			return false;
		}
		if (advanceUponCompletion && !advanceToCheckpoint)
		{
			__instance.ActivateNextQuest(false, false);
		}
		return false;
	}

	[HarmonyPatch("StartQuestForReal")]
	[HarmonyPrefix]
	static bool StartQuestForRealPrefix(QuestOrder __instance, bool fromAutoComplete, bool fromCheckpoint)
	{
		var trv = Traverse.Create(__instance);
		var possibleQuests = trv.Field("possibleQuests").GetValue<List<QuestBase>>();
		QuestBase currentQuest = trv.Field("currentQuest").GetValue<QuestBase>();

		if (possibleQuests.Count > 0)
		{
			for (int i = 0; i < possibleQuests.Count; i++)
			{
				possibleQuests[i].StartQuest(fromAutoComplete, fromCheckpoint);
				SendData.SendQuestInteractable(possibleQuests[i].name, string.Format("StartQuest{0}{1}", fromAutoComplete, fromCheckpoint));
			}
		}
		else
		{
			currentQuest.StartQuest(fromAutoComplete, fromCheckpoint);
			SendData.SendQuestInteractable(currentQuest.name, string.Format("StartQuest{0}{1}", fromAutoComplete, fromCheckpoint));
		}
		RM.tattletail.GetNeeds().OnQuestStarted();
		bool advanceToCheckpoint = trv.Field("advanceToCheckpoint").GetValue<bool>();
		if (currentQuest.updateHUD && !advanceToCheckpoint)
		{
			RM.hud.SetNewQuest(currentQuest.GetDisplayString());
		}
		if (currentQuest.checkpoint != null)
		{
			trv.Field("storedCheckpoint").SetValue(currentQuest.checkpoint.spawnID);
		}

		return false;
	}
}