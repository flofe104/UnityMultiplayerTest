using Server;
using System.Collections;
using System.Collections.Generic;
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
    }

}
