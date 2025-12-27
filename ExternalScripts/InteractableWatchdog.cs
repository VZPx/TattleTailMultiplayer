using HarmonyLib;
using HighlightingSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class InteractableWatchdog : MonoBehaviour
{
	// We store a reference to the interactable we are watching
	public Interactable target;

	private void OnDisable()
	{
		if (target == null) return;

		// Use Traverse to clean up the private fields of the target
		var trv = Traverse.Create(target);

		// Stop Audio
		var holdAudio = trv.Field("holdAudio").GetValue();
		if (holdAudio != null)
		{
			AudioController.Stop("hold_loop");
			trv.Field("holdAudio").SetValue(null);
		}

		// Reset private interaction state
		trv.Field("_isHeld").SetValue(false);
		trv.Field("_holdTimer").SetValue(0f);

		// Turn off highlight
		var highlighter = trv.Field("_highlighter").GetValue<Highlighter>();
		if (highlighter != null)
		{
			highlighter.FlashingOff();
		}

		GS.acceptInput = true;
		Debug.Log($"[MP Mod] Watchdog cleaned up: {target.name}");
	}
}
