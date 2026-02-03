using System;
using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class SteamLobby : MonoBehaviour
{
	public static SteamLobby instance;
	private Callback<LobbyCreated_t> lobbyCreated;
	private Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
	private Callback<LobbyEnter_t> lobbyEntered;
	public static CSteamID lobby;
	public static CSteamID receiver;
	public bool getSecondPlayer;
	public string secondPlayerName = "None";
	private bool allowCreateLobby = true;
	public bool bLobbyCreated;
	private bool isWelcomed;
	public CSteamID currentLobby;
	public bool hasSecondPlayer;

	private void Awake()
	{
		if (SteamLobby.instance != null)
		{
			return;
		}
		SteamLobby.instance = this;
		Debug.Log("Awake on SteamLobby");
	}

	public void Start()
	{
		this.lobbyCreated = Callback<LobbyCreated_t>.Create(new Callback<LobbyCreated_t>.DispatchDelegate(this.OnLobbyCreated));
		this.gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(new Callback<GameLobbyJoinRequested_t>.DispatchDelegate(this.OnGameLobbyJoinRequested));
		this.lobbyEntered = Callback<LobbyEnter_t>.Create(new Callback<LobbyEnter_t>.DispatchDelegate(this.OnLobbyEntered));
		Networking.InitializeClientData();
	}

	public void OnLobbyCreated(LobbyCreated_t callback)
	{
		if (callback.m_eResult == EResult.k_EResultOK)
		{
			SteamLobby.lobby = new CSteamID(callback.m_ulSteamIDLobby);
			Debug.Log("Lobby created successefully");
		}
	}

	public void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
	{
		Debug.Log("Request to join lobby");
		SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
		this.getSecondPlayer = true;
		this.bLobbyCreated = true;
		MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Leave Lobby";
		MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().Rebuild();
		MainMenuPatch.inviteFriendsButton.GetComponent<Button>().interactable = true;
	}

	public void OnLobbyEntered(LobbyEnter_t callback)
	{
		Debug.Log("You have successefully joined the lobby");
		SteamLobby.lobby = new CSteamID(callback.m_ulSteamIDLobby);
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.lobby);
		for (int i = 0; i < numLobbyMembers; i++)
		{
			SteamLobby.receiver = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobby, i);
			SendData.SendWelcome("Player " + SteamFriends.GetPersonaName() + " successefully connected");
		}
	}

	public void CreateLobby()
	{
		if (!this.bLobbyCreated)
		{
			SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 2);
			this.getSecondPlayer = true;
			this.bLobbyCreated = true;
			MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Leave Lobby";
			MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().Rebuild();
			MainMenuPatch.inviteFriendsButton.GetComponent<Button>().interactable = true;
			return;
		}
		SteamMatchmaking.LeaveLobby(SteamLobby.lobby);
		MainMenuPatch.inviteFriendsButton.GetComponent<Button>().interactable = false;
		MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Create Lobby";
		MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().Rebuild();
		if (!MainMenuPatch.ConnectionsMP.text.Contains("None"))
		{
			MainMenuPatch.ConnectionsMP.text = "Connected: None";
			MainMenuPatch.ConnectionsMP.Rebuild();
		}
		this.getSecondPlayer = false;
		this.bLobbyCreated = false;
	}

	public void InviteFriends()
	{
		if (this.bLobbyCreated)
		{
			SteamFriends.ActivateGameOverlayInviteDialog(SteamLobby.lobby);
		}
	}

	public void Update()
	{
		Networking.ListenData();
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.lobby);
		if (numLobbyMembers > 1)
		{
			for (int i = 0; i < numLobbyMembers; i++)
			{
				if (SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobby, i) != SteamUser.GetSteamID())
				{
					this.hasSecondPlayer = true;
				}
			}
		}
		else
		{
			this.hasSecondPlayer = false;
		}
		if (this.getSecondPlayer)
		{
			CSteamID csteamID = CSteamID.Nil;
			SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobby, 1);
			int numLobbyMembers2 = SteamMatchmaking.GetNumLobbyMembers(SteamLobby.lobby);
			for (int j = 0; j < numLobbyMembers2; j++)
			{
				CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex(SteamLobby.lobby, j);
				if (lobbyMemberByIndex != SteamUser.GetSteamID())
				{
					csteamID = lobbyMemberByIndex;
				}
			}
			if (csteamID != CSteamID.Nil)
			{
				SteamLobby.receiver = csteamID;
				if (SteamFriends.GetFriendPersonaName(csteamID) != null)
				{
					this.secondPlayerName = SteamFriends.GetFriendPersonaName(csteamID);
				}
				else
				{
					Debug.Log("Error on steamlobby");
					this.secondPlayerName = "None";
				}
				if (!this.isWelcomed)
				{
					SendData.SendWelcome("Welcome to the lobby");
					this.isWelcomed = true;
				}
				MainMenuPatch.ConnectionsMP.text = "Connected: " + SteamLobby.instance.secondPlayerName;
				MainMenuPatch.ConnectionsMP.Rebuild();
				this.getSecondPlayer = false;
				return;
			}
		}
		else
		{
			//this.secondPlayerName = "None"; (Does this cause a visual bug when removed?)
		}
	}

	public void CheckIfLobby()
	{
		try
		{
			if (this.bLobbyCreated)
			{
				this.bLobbyCreated = true;
				MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Leave Lobby";
				MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().Rebuild();
				MainMenuPatch.inviteFriendsButton.GetComponent<Button>().interactable = true;
			}
			else
			{
				MainMenuPatch.inviteFriendsButton.GetComponent<Button>().interactable = false;
				MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().text = "Create Lobby";
				MainMenuPatch.MPHolder.transform.FindChild("CreateLobby").transform.GetChild(0).GetComponent<SuperTextMesh>().Rebuild();
				if (!MainMenuPatch.ConnectionsMP.text.Contains("None"))
				{
					MainMenuPatch.ConnectionsMP.text = "Connected: None";
					MainMenuPatch.ConnectionsMP.Rebuild();
				}
				this.bLobbyCreated = false;
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}
}
