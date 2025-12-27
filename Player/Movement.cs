using System;
using UnityEngine;

// Token: 0x020001E9 RID: 489
public class Movement : MonoBehaviour
{
	private CharacterController _CharacterController;
	public Vector3 _PlayerObjectPosition;
	public float _PlayerObjectRotation;
	public Vector3 _PlayerObjectSpeed;

	public void Start()
	{
		_CharacterController = GetComponent<CharacterController>();
	}

	public void Update()
	{
		if (transform.position != _PlayerObjectPosition)
		{
			transform.position = _PlayerObjectPosition;
		}
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 
			_PlayerObjectRotation, transform.rotation.eulerAngles.z);
	}

	public void FixedUpdate()
	{
		_CharacterController.Move(_PlayerObjectSpeed);
	}
}
