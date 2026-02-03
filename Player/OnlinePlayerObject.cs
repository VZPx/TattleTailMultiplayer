using System;
using UnityEngine;

public class OnlinePlayerObject : MonoBehaviour
{
	public static OnlinePlayerObject instance;
	public Movement _Movement;
	public GameObject flashLight, Tattletail, CamTransform;
	public MeshRenderer renderer;

	public void Start()
	{
		this._Movement = base.gameObject.AddComponent<Movement>();
		renderer = gameObject.GetComponent<MeshRenderer>();	
		instance = this;
	}

	public void Update()
	{
	}

	public void FixedUpdate()
	{
	}

	void OnLevelWasLoaded(int levelIndex)
	{
		renderer.enabled = true;
	}
}
