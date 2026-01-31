using BepInEx;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;


[BepInPlugin("TattleTailMP", "TattleTail Multiplayer MOD", "0.9.6")]
public class Main : BaseUnityPlugin
{
	internal static new ManualLogSource Logger;
    void Awake()
    {
		Logger = base.Logger;
        Logger.LogInfo("TattleTail MP Mod Loaded!");
        Logger.LogInfo("Made by VZP and MiniNova");

		var harmony = new Harmony("com.vzp.tattletailmp");
		harmony.PatchAll();
	}

	void Update()
	{
		
	}

	void OnApplicationQuit()
	{
	}
}
