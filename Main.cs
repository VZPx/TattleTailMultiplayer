using BepInEx;
using UnityEngine;
using HarmonyLib;


[BepInPlugin("TattleTailMP", "TattleTail Multiplayer MOD", "0.9.6")]
public class Main : BaseUnityPlugin
{
    void Awake()
    {
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
