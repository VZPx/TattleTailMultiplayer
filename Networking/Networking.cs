using System;
using System.Collections.Generic;
using Steamworks;

public class Networking
{
	private static Dictionary<int, Networking.PacketHandler> packetHandlers;

	private delegate void PacketHandler(Packet _packet);

	public static void ListenData()
	{
		uint num;
		while (SteamNetworking.IsP2PPacketAvailable(out num, 0))
		{
			byte[] array = new byte[num];
			uint num2;
			CSteamID csteamID;
			if (SteamNetworking.ReadP2PPacket(array, num, out num2, out csteamID, 0))
			{
				Networking.HandleReceivedData(array);
			}
		}
	}

	public static void SendUDPData(Packet packet)
	{
		byte[] array = packet.ToArray();
		SteamNetworking.SendP2PPacket(SteamLobby.receiver, array, (uint)array.Length, 0, 0);
	}

	public static void SendTCPData(Packet packet)
	{
		byte[] array = packet.ToArray();
		SteamNetworking.SendP2PPacket(SteamLobby.receiver, array, (uint)array.Length, EP2PSend.k_EP2PSendReliable, 0);
	}

	public static void HandleReceivedData(byte[] _data)
	{
		using (Packet packet = new Packet(_data))
		{
			int num = packet.ReadInt(true);
			Networking.packetHandlers[num](packet);
		}
	}

	public static void InitializeClientData()
	{
		Dictionary<int, Networking.PacketHandler> dictionary = new Dictionary<int, Networking.PacketHandler>();
		dictionary.Add(0, new Networking.PacketHandler(HandleData.WelcomeReceived));
		dictionary.Add(1, new Networking.PacketHandler(HandleData.MovementReceived));
		dictionary.Add(2, new Networking.PacketHandler(HandleData.InteractionReceived));
		dictionary.Add(3, new Networking.PacketHandler(HandleData.InputReceived));
		dictionary.Add(4, new Networking.PacketHandler(HandleData.LevelTransition));
		dictionary.Add(5, new Networking.PacketHandler(HandleData.CommericalAction));
		dictionary.Add(6, new Networking.PacketHandler(HandleData.HandleSwap));
		dictionary.Add(7, new Networking.PacketHandler(HandleData.HasInputtedItem));
		dictionary.Add(8, new Networking.PacketHandler(HandleData.HandleTrigger));
		dictionary.Add(9, new Networking.PacketHandler(HandleData.HandleQuestTrigger));
		dictionary.Add(10, new Networking.PacketHandler(HandleData.HandleQuestInteractable));
		Networking.packetHandlers = dictionary;
	}
}
