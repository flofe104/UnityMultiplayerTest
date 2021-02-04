using Server;
using ServerData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{

    public static void Welcome(Packet p)
    {
        string msg = p.ReadString();
        int id = p.ReadInt();

        Debug.Log(msg);
        Client.instance.myId = id;

        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
    }

    public static void SpawnPlayer(Packet p)
    {
        PlayerData data = (PlayerData)p.ReadObject();

        GameManager.instance.SpawnPlayer(data);
    }

    public static void PlayerPosition(Packet p)
    {
        int id = p.ReadInt();
        Vector3 pos = p.ReadObject<Vector3>();
        GameManager.players[id].transform.position = pos;
    }

    public static void PlayerRotation(Packet p)
    {
        int id = p.ReadInt();
        Quaternion rot = p.ReadObject<Quaternion>();
        GameManager.players[id].transform.rotation = rot;
    }

    public static void PlayerDisconnected(Packet p)
    {
        int id = p.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }
}
