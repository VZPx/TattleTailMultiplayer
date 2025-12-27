using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

public class ReadData : MonoBehaviour
{
	private CharacterController _CharacterController;
	private FirstPersonController _FirstPersonController;
	private FieldInfo _MoveDir;
	private FieldInfo _GravityPower;
	private FieldInfo _Input;
	private Vector3 _PlayerPosition;
	private float _PlayerRotation;
	private Vector3 _Motion;
	private Vector3 _CameraPosition;
	private Vector3 _CameraForward;
	private float _InputY;
	private Camera _CameraContainer;
	private LayerMask mask;

	public void Start()
	{
		this.mask = -1;
		this._CharacterController = base.GetComponent<CharacterController>();
		this._FirstPersonController = base.GetComponent<FirstPersonController>();
		this._MoveDir = typeof(FirstPersonController).GetField("m_MoveDir", (BindingFlags)36);
		this._Input = typeof(FirstPersonController).GetField("m_Input", (BindingFlags)36);
		this._CameraContainer = Traverse.Create(this._FirstPersonController)
			.Field("m_Camera").GetValue<Camera>();
	}

	public void Update()
	{
	}

	public void FixedUpdate()
	{
		this.ReadMovement();
		SendData.SendMovement(this._PlayerPosition, this._PlayerRotation, this._Motion);
		this.ReadInput();
		SendData.SendInput(this._InputY);
	}

	public void ReadMovement()
	{
		this._PlayerPosition = base.transform.position;
		this._PlayerRotation = base.transform.rotation.eulerAngles.y;
		Vector3 vector = (Vector3)this._MoveDir.GetValue(this._FirstPersonController);
		this._Motion = vector * Time.fixedDeltaTime + Vector3.up;
	}

	public void ReadInteraction()
	{
		this._CameraPosition = this._CameraContainer.transform.position;
		this._CameraForward = this._CameraContainer.transform.forward;
	}

	public void ReadInput()
	{
		this._InputY = ((Vector2)this._Input.GetValue(this._FirstPersonController)).y;
	}

	
}
