﻿using System;
using System.Net.Sockets;
using System.Threading;
using AliveChessClient.NetLayer.Main;
using AliveChessLibrary.Commands;
using AliveChessLibrary.Net;

namespace AliveChessClient.NetLayer.Transport
{
    public class SocketTransport
    {
        private Socket socket;
        private CommandPool commandPool;
        private ConnectionInfo conn;
        private NerworkDataStream receivedData;

       // private Queue<ArraySegment<byte>> segments;
        private object syncSegments = new object();
       // private Configuration cfg;

        public SocketTransport(Socket socket, CommandPool commandPool)
        {
           // AliveChessLog.Clear("log.txt");
            this.socket = socket;
            this.commandPool = commandPool;
            this.conn = new ConnectionInfo(socket);
            this.receivedData = new NerworkDataStream();
        }

        public void Decode()
        {
            while (true)
            {
                if (receivedData.IsReady)
                {
                    BytePackage data = receivedData.Get();
                    if (data != null)
                    {
                        //AliveChessLog.Write("log.txt", data.Length.ToString() + " bytes was received");
                        ICommand command = ProtoBufferCodec.Decode(data);
                        if (command != null)
                        {
                            //AliveChessLog.Write("log.txt", "decoded");
                            commandPool.Enqueue(command);
                        }
                       // else AliveChessLog.Write("log.txt", "error has occured");
                    }
                }

                Thread.Sleep(10);
            }
        }

        /// <summary>
        /// receive data from server. This method can receive 8192 bytes.
        /// </summary>
        public void Receive()
        {
            int byteData = 0;
            byte[] buffer = new byte[1024]; // 8192 bytes

            while (true)
            {
                if (socket.Connected)
                {
                    try
                    {
                        byteData = socket.Receive(buffer);

                        if (byteData > 0)
                        {
                            byte[] data = new byte[byteData];
                            Array.Copy(buffer, data, byteData);

                            receivedData.Put(data);
                        }
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }
        }

        public void Send<T>(T command) where T : ICommand
        {
            byte[] data = ProtoBufferCodec.Encode<T>(command);
            if (data != null)
                socket.Send(data);
        }

        public Socket Socket
        {
            get { return socket; }
            set { socket = value; }
        }
    }
}