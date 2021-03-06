﻿using Server;
using ServerData;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend
{
    
    private static void SendTCPData(Packet p)
    {
        p.WriteLength();
        Client.instance.tcp.SendData(p);
    }

    private static void SendUDPData(Packet p)
    {
        p.WriteLength();
        Client.instance.udp.SendData(p);
    }

    public static void SendPlayerInput(InputData inputData)
    {
        using (Packet p = new Packet((int)ClientPackets.playerMovement))
        {
            p.Write(inputData);
            SendUDPData(p);
        }  
    }

    public static void WelcomeReceived()
    {
        using(Packet p = new Packet((int)ClientPackets.welcomeReceived))
        {
            p.Write(Client.instance.myId);
            p.Write(UIManager.instance.usernameField.text);

            SendTCPData(p);
        }
    }

}
