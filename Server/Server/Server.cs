using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;


namespace Server
{
    class Server
    {

        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }

        public static Dictionary<int, Client> clients;

        public delegate void PacketHandler(int fromClient, Packet p);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine($"Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Server started on Port {Port}.");
        }

        private static void TCPConnectCallback(IAsyncResult result)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"Incomming connection from {client.Client.RemoteEndPoint}...");
            bool assigned = false;
            for (int i = 1; i <= MaxPlayers && !assigned; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    assigned = true;
                    clients[i].tcp.Connect(client);
                    Console.WriteLine($"{client.Client.RemoteEndPoint} connected sucessful");
                }
            }
            if (!assigned)
            {
                Console.WriteLine($"{client.Client.RemoteEndPoint} failed to connect: Server full");
            }
        }

        private static void InitializeServerData()
        {
            clients = new Dictionary<int, Client>(MaxPlayers);
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeRecieved}
            };
            Console.WriteLine("Initialized packets.");
        }
    }
}
