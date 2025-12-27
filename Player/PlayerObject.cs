using System;
using UnityEngine;

public class PlayerObject : MonoBehaviour
{
	public static PlayerObject instance;
	public Movement _Movement;
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
