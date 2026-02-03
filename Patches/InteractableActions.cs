using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(InteractableActions), "PlayActions")]
public class InteractableActionsPatch
{
	[HarmonyPrefix]
	static bool Prefix(InteractableActions __instance, Interactable caller, bool fromAutoComplete)
	{
		for (int i = 0; i < __instance.interactActions.Length; i++)
		{
			switch (__instance.interactActions[i])
			{
				case InteractableActions.Actions.FlashlightCarry:
					GS.Flashlight(true);
					break;
				case InteractableActions.Actions.FlashlightUncarry:
					GS.Flashlight(false);
					break;
				case InteractableActions.Actions.TattletailCarry:
					GS.Tattletail(true, false);
					break;
				case InteractableActions.Actions.TattletailUncarry:
					GS.Tattletail(false, false);
					break;
				case InteractableActions.Actions.MamaActivate:
					RM.mamaManager.CreateMamaSpawner();
					break;
				case InteractableActions.Actions.MamaDeactivate:
					RM.mamaManager.RemoveExistingMama();
					break;
				case InteractableActions.Actions.CompleteLevel:
					GameManager.OnLevelComplete();
					break;
				case InteractableActions.Actions.TattletailSetInBox:
					GS.tattletailInBox = true;
					break;
				case InteractableActions.Actions.GenerateNoise:
					if (!fromAutoComplete)
					{
						RM.mamaManager.OnNoise(caller.transform.position, true);
					}
					break;
				case InteractableActions.Actions.CompleteQuest:
					if (!fromAutoComplete)
					{
						RM.questOrder.CompleteCurrentQuest(caller.gameObject);
					}
					break;
				case InteractableActions.Actions.FeedTattletail:
					RM.tattletail.GetNeeds().Feed(true);
					break;
				case InteractableActions.Actions.GroomTattletail:
					RM.tattletail.GetNeeds().Groom(true);
					break;
				case InteractableActions.Actions.TankHunger:
					RM.tattletail.GetNeeds().TankHunger();
					break;
				case InteractableActions.Actions.TankGrooming:
					RM.tattletail.GetNeeds().TankGrooming();
					break;
				case InteractableActions.Actions.LockHunger:
					RM.tattletail.GetNeeds().LockHunger();
					break;
				case InteractableActions.Actions.LockGrooming:
					RM.tattletail.GetNeeds().LockDirtiness();
					break;
				case InteractableActions.Actions.UnlockHunger:
					RM.tattletail.GetNeeds().UnlockHunger();
					break;
				case InteractableActions.Actions.UnlockGrooming:
					RM.tattletail.GetNeeds().UnlockDirtiness();
					break;
				case InteractableActions.Actions.StartQuesting:
					if (!fromAutoComplete)
					{
						RM.questOrder.StartQuesting();
					}
					break;
				case InteractableActions.Actions.ChargeTattletail:
					RM.tattletail.GetNeeds().Charge(true);
					break;
				case InteractableActions.Actions.LowerBattery:
					RM.tattletail.GetNeeds().SetLowBattery();
					break;
				case InteractableActions.Actions.LockBattery:
					RM.tattletail.GetNeeds().LockBattery();
					break;
				case InteractableActions.Actions.UnlockBattery:
					RM.tattletail.GetNeeds().UnlockBattery();
					break;
				case InteractableActions.Actions.KillLights:
					if (!fromAutoComplete)
					{
						RM.lightsController.RequestLightsOut(null);
					}
					break;
				case InteractableActions.Actions.RestoreLights:
					if (!fromAutoComplete)
					{
						RM.lightsController.RequestLightsOn(null);
					}
					break;
				case InteractableActions.Actions.LowerHunger:
					RM.tattletail.GetNeeds().SetLowHunger();
					break;
				case InteractableActions.Actions.LowerGrooming:
					RM.tattletail.GetNeeds().SetLowGrooming();
					break;
				case InteractableActions.Actions.ForceMamaSpawn:
					if (!fromAutoComplete)
					{
						RM.mamaManager.StartActivation();
					}
					break;
				case InteractableActions.Actions.MaxBars:
					RM.tattletail.GetNeeds().MaxAllStats();
					break;
				case InteractableActions.Actions.RecolorCarryableTattletail:
					RM.recolorManager.RecolorObject(RM.tattletail.tattletail, RecolorType.STORED_COLOR);
					break;
				case InteractableActions.Actions.MakeSpawnerManual:
					RM.mamaManager.MakeSpawnerManual();
					break;
				case InteractableActions.Actions.SetChatterStateStandard:
					RM.tattletail.SetChatterState(TattletailChatterState.STANDARD);
					break;
				case InteractableActions.Actions.SetChatterStateTired:
					RM.tattletail.SetChatterState(TattletailChatterState.SLEEPY);
					break;
				case InteractableActions.Actions.SetChatterStateSing:
					RM.tattletail.SetChatterState(TattletailChatterState.SINGING);
					break;
				case InteractableActions.Actions.TattletailCarrySilent:
					GS.Tattletail(true, true);
					break;
				case InteractableActions.Actions.KillScreenshake:
					RM.fpsController.RequestScreenshake(1f, 0.2f);
					break;
				case InteractableActions.Actions.SetChatterStateLocked:
					RM.tattletail.SetChatterState(TattletailChatterState.LOCKED);
					break;
				case InteractableActions.Actions.AdvanceToSpecificLevel:
					GameManager.PlayLevel(__instance.customLevelEnum);
					if (!HandleData.isNetworkPacket)
					{
						SendData.SendLevelTransition(__instance.customLevelEnum.ToString());
					}
					break;
				case InteractableActions.Actions.SetTattletailType_Standard:
					RM.tattletail.SetTattletailType(TattletailController.TattletailType.STANDARD);
					break;
				case InteractableActions.Actions.SetTattletailType_Educational:
					RM.tattletail.SetTattletailType(TattletailController.TattletailType.EDUCATIONAL);
					break;
				case InteractableActions.Actions.EnterTVMode:
					caller.GetComponent<TVMode>().EnterTVMode(-1f);
					break;
				case InteractableActions.Actions.FinalJoke:
					GameObject.Find("Jumpscare").GetComponent<QuestWaydriveEnding>().StartFinalJoke();
					break;
				case InteractableActions.Actions.DarkRoomFlow_1:
					caller.transform.root.FindChild("FakeBoxTTTrigger").GetComponent<QuestTriggerWaydriveFinale>().WhosThere();
					break;
				case InteractableActions.Actions.DarkRoomFlow_2:
					caller.transform.root.FindChild("FakeBoxTTTrigger").GetComponent<QuestTriggerWaydriveFinale>().StartPunchline();
					break;
				case InteractableActions.Actions.JokeFlow_1:
					caller.transform.parent.parent.FindChild("JokeTrigger").GetComponent<TriggerBananaJoke>().WhosThere();
					break;
				case InteractableActions.Actions.JokeFlow_2:
					caller.transform.parent.parent.FindChild("JokeTrigger").GetComponent<TriggerBananaJoke>().StartPunchline();
					break;
				case InteractableActions.Actions.FlickerLights:
					if (!fromAutoComplete)
					{
						RM.lightsController.RequestLightsFlicker(null);
					}
					break;
				case InteractableActions.Actions.CycleFacts:
					caller.GetComponent<QuestCycleFacts>().NewFact();
					break;
				case InteractableActions.Actions.Credits:
					SceneManager.LoadScene("Credits", 0);
					break;
				case InteractableActions.Actions.PreFinalJoke:
					GameObject.Find("Jumpscare").GetComponent<QuestWaydriveEnding>().StartPreFinalJoke();
					break;
				case InteractableActions.Actions.SetChatterStateShiver:
					RM.tattletail.SetChatterState(TattletailChatterState.SHIVER);
					break;
			}
		}
		return false;
	}
}