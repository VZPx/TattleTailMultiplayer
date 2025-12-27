using UnityEngine;

public class MainMenuMPHelper : MonoBehaviour
{
	private bool mamaFound;
	private GameObject mamaEvil;

	void Update()
	{
		if (mamaEvil == null)
		{
			mamaEvil = GameObject.Find("mamaEvil");
		}

		if (mamaEvil != null)
		{
			// Access the static rotation from your Patch class
			mamaEvil.transform.rotation = Quaternion.Lerp(
				mamaEvil.transform.rotation,
				MainMenuPatch.mamaRotation,
				5f * Time.deltaTime
			);
		}
	}
}