using System;
using UnityEngine;

public class OnlinePlayerObject : MonoBehaviour
{
	public static OnlinePlayerObject instance;
	public Movement _Movement;
	public GameObject flashLight, Tattletail, CamTransform;

	public void Start()
	{
		this._Movement = base.gameObject.AddComponent<Movement>();
		instance = this;
	}

	public void Update()
	{
	}

	public void FixedUpdate()
	{
	}
}
