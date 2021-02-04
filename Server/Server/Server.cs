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
        private static UdpClient udpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine($"Starting server...");
            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UdpReceiveCallback, null);


            Console.WriteLine($"Server started on Port {Port}.");
        }

        private static void UdpReceiveCallback(IAsyncResult ar)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpListener.EndReceive(ar, ref clientEndPoint);
                udpListener.BeginReceive(UdpReceiveCallback, null);

                if(data.Length >= 4)
                {
                    using(Packet p = new Packet(data))
                    {
                        int clientId = p.ReadInt();
                        if(!clients.ContainsKey(clientId))
                        {
                            return;
                        }

                        if(clients[clientId].udp.endPoint == null)
                        {
                            clients[clientId].udp.Connect(clientEndPoint);
                            return;
                        }

                        if (clients[clientId].udp.endPoint.ToString() == clientEndPoint.ToString())
                        {
                            clients[clientId].udp.HandleData(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error recieving data via udp: {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet p)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(p.ToArray(), p.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data via udp: {ex}");
            }
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
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeRecieved } ,
                { (int)ClientPackets.playerMovement, ServerHandle.HandlePlayerInput }
                
            };
            Console.WriteLine("Initialized packets.");
        }

   
    }
}
