using BepInEx;
using BepInEx.Logging;
using System;
using UnityEngine;

public class SendData : MonoBehaviour
{
	public static void SendWelcome(string msg)
	{
		using (Packet packet = new Packet(0))
		{
			packet.Write(msg);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendMovement(Vector3 pos, float rot, Vector3 speed)
	{
		using (Packet packet = new Packet(1))
		{
			packet.Write(pos);
			packet.Write(rot);
			packet.Write(speed);
			Networking.SendUDPData(packet);
		}
	}

	public static void SendInteraction(Vector3 origin, Vector3 direction, bool state, string objName)
	{
		using (Packet packet = new Packet(2))
		{
			packet.Write(origin);
			packet.Write(direction);
			packet.Write(state);
			packet.Write(objName);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendInput(float input)
	{
		using (Packet packet = new Packet(3))
		{
			packet.Write(input);
			Networking.SendUDPData(packet);
		}
	}

	public static void SendLevelTransition(string level)
	{
		Main.Logger.LogInfo($"Sending to level: {level}");
		using (Packet packet = new Packet(4))
		{
			packet.Write(level);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendCommercialAction(string cam)
	{
		using (Packet packet = new Packet(5))
		{
			packet.Write(cam);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendSwap(string objName, Vector3 newPosition, Quaternion newRotation)
	{
		using (Packet packet = new Packet(6))
		{
			packet.Write(objName);
			packet.Write(newPosition);
			packet.Write(newRotation);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendInput(bool inputted)
	{
		using (Packet packet = new Packet(7))
		{
			packet.Write(inputted);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendTrigger(string objName, bool manualTrigger, bool autoComplete)
	{
		using (Packet packet = new Packet(8))
		{
			Debug.Log($"Sent {objName} Trigger with parameters \n" +
				$"{manualTrigger}, {autoComplete}. -MP");
			packet.Write(objName);
			packet.Write(manualTrigger);
			packet.Write(autoComplete);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendQuestTrigger(string objName, bool autoComplete, bool questComplete)
	{
		using (Packet packet = new Packet(9))
		{
			Debug.Log($"Sent Quest Trigger {objName} with parameters \n" +
				$"{autoComplete}, {questComplete}. -MP");
			packet.Write(objName);
			packet.Write(autoComplete);
			packet.Write(questComplete);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendQuestInteractable(string objName, string methodName)
	{
		using (Packet packet = new Packet(10))
		{
			Debug.Log($"Sent Quest Interactable: {objName}\n" +
				$"Called {methodName} -MP");
			packet.Write(objName);
			packet.Write(methodName);
			Networking.SendTCPData(packet);
		}
	}

	public static void SendPlayerStatus(string playerName, bool isAlive)
	{
		using (Packet packet = new Packet(11))
		{
			Debug.Log($"Sent Player Status: {playerName},\n" +
				$"isAlive: {isAlive} -MP");
			packet.Write(playerName);
			packet.Write(isAlive);
			Networking.SendTCPData(packet);
		}
	}
}
