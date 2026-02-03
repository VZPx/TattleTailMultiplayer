using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RevoltMain : MonoBehaviour
{
	public static RevoltMain instance;
	public FirstPersonController _Player;
	public GameObject _PlayerObject;
	public Animator _PlayerObjectAnimator;
	public CharacterController _PlayerObjectCC;
	public bool getPlayer = true;
	private string currentScene;
	public List<string> deadPlayers = new List<string>(); //used for spectating

	public void Start()
	{
		SteamAPI.Init();
		RevoltMain.instance = this;
		currentScene = "menu";
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public void Update()
	{
		if (getPlayer)
		{
			if (_PlayerObject != null)
			{
				getPlayer = false;
				return;
			}
			GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
			gameObject.transform.localScale = new Vector3(0.54922f, 0.54922f, 0.54922f);
			gameObject.layer = 9;
			gameObject.name = "SecondPlayer";
			if (gameObject != null)
			{
				global::UnityEngine.Object.DontDestroyOnLoad(gameObject);
				if (gameObject.GetComponent<CharacterController>() == null)
				{
					gameObject.AddComponent<CharacterController>();
				}
				_PlayerObject = gameObject;
				_PlayerObjectCC = gameObject.GetComponent<CharacterController>();
				_PlayerObject.AddComponent<OnlinePlayerObject>();
				return;
			}
		}
		if ((!SteamLobby.instance.hasSecondPlayer || currentScene == "Credits" || currentScene == "menu") && _PlayerObject != null)
		{
			_PlayerObject.SetActive(false);
		}
		if (_PlayerObject != null && SteamLobby.instance.hasSecondPlayer && currentScene != "Credits" && currentScene != "menu")
		{
			_PlayerObject.SetActive(true);
		}
	}

	public void FixedUpdate()
	{
		if (_Player == null)
		{
			_Player = global::UnityEngine.Object.FindObjectOfType<FirstPersonController>();
			if (_Player != null)
			{
				_Player.gameObject.AddComponent<ReadData>();
			}
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
	{
		currentScene = SceneManager.GetActiveScene().name;
		Debug.Log("new scene loaded -VZP");		
	}
}
