using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Client
    {
        public static int dataBufferSize = 4096;
        public int id;
        public TCP tcp;

        public Client(int clientId)
        {
            id = clientId;
            tcp = new TCP(id);
        }

        public class TCP
        {

            public TCP(int id)
            {
                this.id = id;
            }

            public void SendData(Packet p)
            {
                try
                {
                    if(socket != null)
                    {
                        stream.BeginWrite(p.ToArray(),0, p.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending data to player {id} via TCP: {ex}");
                }
            }

            public TcpClient socket;

            private NetworkStream stream;

            private Packet recievedData;

            private byte[] recieveBuffer;
            private readonly int id;

            public void Connect(TcpClient socket)
            {
                this.socket = socket;
                socket.ReceiveBufferSize = dataBufferSize;
                stream = socket.GetStream();


                recievedData = new Packet();
                recieveBuffer = new byte[dataBufferSize];
                stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallBack, null);

                ServerSend.Welcome(id, "Welcome to the server");
            }

            private void ReceiveCallBack(IAsyncResult result)
            {
                try
                {
                    int byteLength = stream.EndRead(result);
                    if(byteLength > 0)
                    {
                        byte[] data = new byte[byteLength];
                        Array.Copy(recieveBuffer, data, byteLength);

                        recievedData.Reset(HandleData(data));
                        stream.BeginRead(recieveBuffer, 0, dataBufferSize, ReceiveCallBack, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on recieve callback {ex}");
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
                            Server.packetHandlers[packetId](id,p);
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

        }



    }
}
