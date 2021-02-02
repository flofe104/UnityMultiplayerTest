using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class ServerHandle
    {

        public static void WelcomeRecieved(int _fromClient, Packet p)
        {
            int clientIdCheck = p.ReadInt();
            string username = p.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient} ({username})");

            if(_fromClient != clientIdCheck)
            {
                Console.WriteLine($" Player \"{username}\" (ID:{_fromClient}) has assumed the wrong client ID ({clientIdCheck})!");
            }
        }

    }
}
