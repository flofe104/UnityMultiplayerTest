using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine;
using Server;

public class Client : MonoBehaviour
{
    public static Client instance;
    public static int dataBufferSize = 4096;

    public string ip = "127.0.0.1";
    public int port = 24680;
    public int myId = 0;
    public TCP tcp;
    public UDP udp;

    private delegate void PacketHandler(Packet p);
    private static Dictionary<int, PacketHandler> packetHandlers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exsits, destroy object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
        udp = new UDP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            {(int)ServerPackets.welcome, ClientHandle.Welcome},
                 {(int)ServerPackets.udpTest, ClientHandle.UdpTest }

        };
        Debug.Log("Initialized packets.");
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet recievedData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize
            };
            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(instance.ip, instance.port, ConnectCallback, socket);
        }

        private void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                return;
            }
            stream = socket.GetStream();

            recievedData = new Packet();

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int _byteLength = stream.EndRead(result);
                if (_byteLength <= 0)
                {
                    return;
                }
                byte[] data = new byte[_byteLength];
                Array.Copy(receiveBuffer, data, _byteLength);

                recievedData.Reset(HandleData(data));

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {

            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;
            recievedData.SetBytes(data);
            if (recievedData.UnreadLength() >= 4)
            {
                packetLength = recievedData.ReadInt();
                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= recievedData.UnreadLength())
            {
                byte[] packetBytes = recievedData.ReadBytes(packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet p = new Packet(packetBytes))
                    {
                        int packetId = p.ReadInt();
                        packetHandlers[packetId](p);
                    }
                });

                packetLength = 0;
                if (recievedData.UnreadLength() >= 4)
                {
                    packetLength = recievedData.ReadInt();
                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            return packetLength <= 1;
        }

        public void SendData(Packet p)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(p.ToArray(), 0, p.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error while sending data: {ex}");
            }
        }
    }

    public class UDP
    {
        public UdpClient socket;
        public IPEndPoint endPoint;

        public UDP()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(instance.ip), instance.port);
        }

        public void Connect(int localPort)
        {
            socket = new UdpClient(localPort);
            socket.Connect(endPoint);
            socket.BeginReceive(RecieveCallBack, null);

            using (Packet p = new Packet())
            {
                SendData(p);
            }
        }

        public void SendData(Packet p)
        {
            try
            {
                p.InsertInt(instance.myId);
                if (socket != null)
                {
                    socket.BeginSend(p.ToArray(), p.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to server via UDP: {ex}");
            }
        }

        private void RecieveCallBack(IAsyncResult ar)
        {
            try
            {
                byte[] data = socket.EndReceive(ar, ref endPoint);
                socket.BeginReceive(RecieveCallBack, null);

                if (data.Length >= 4)
                {
                    HandleData(data);

                }
            }
            catch (Exception ex)
            {

            }
        }

        private void HandleData(byte[] data)
        {
            using (Packet p = new Packet(data))
            {
                int packetLength = p.ReadInt();
                data = p.ReadBytes(packetLength);
            };
            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet p = new Packet(data))
                {
                    int packetId = p.ReadInt();
                    packetHandlers[packetId](p);
                };
            });
        }
    }

}
