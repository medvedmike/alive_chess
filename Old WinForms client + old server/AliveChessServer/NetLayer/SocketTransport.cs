﻿using System;
using System.Net;
using System.Net.Sockets;
using AliveChessLibrary.Net;
using AliveChessServer.LogicLayer;

namespace AliveChessServer.NetLayer
{
    public class SocketTransport
    {
        private CommandPool _commandPool;
        private AliveChessLogger _logger;
        private ConnectionPool _connections;

        public SocketTransport(CommandPool commands, AliveChessLogger logger)
        {
            this._logger = logger;
            this._commandPool = commands;
            this._connections = new ConnectionPool(this);
        }

        public void Send(byte[] data, Socket socket)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCallback), socket);
            }
            catch (Exception ex)
            {
                _logger.Add(ex.Message);
            }
            
        }

        public void AddConnection(Socket socket)
        {
            // ожидание при отключении
            LingerOption linger = new LingerOption(true, 1);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, linger);

            // создаем сессию
            ConnectionInfo info = new ConnectionInfo(socket);
            _connections.Add(info);

            _logger.Add(socket.RemoteEndPoint.ToString() + " is connected");

            // начинаем прием сообщений
            Receive(info);
        }

        public void RemoveConnection(ConnectionInfo conn)
        {
            // прекращаем прием/передачу данных
            conn.Socket.Shutdown(SocketShutdown.Both);
            
            _logger.Add(String.Concat(conn.Socket.RemoteEndPoint.ToString(), " is disconnected"));

            // закрываем сокет
            conn.Socket.Close();

            // удаляем соединение
            _connections.Remove(conn);
        }

        public ConnectionInfo SearchConnection(IPEndPoint ipEndPoint)
        {
            foreach (ConnectionInfo info in _connections)
            {
                if (info.Equals(ipEndPoint))
                    return info;
            }
            return null;
        }

        private void Receive(ConnectionInfo connection)
        {
            try
            {
                connection.Socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, 0,
                    new AsyncCallback(ReceiveCallback), connection);
            }
            catch (SocketException ex) 
            { 
                _logger.Add(ex.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            ConnectionInfo connection = (ConnectionInfo)result.AsyncState;
            Socket handler = connection.Socket;
            int byteData = 0;

            try
            {
                byteData = handler.EndReceive(result);
            }
            catch (SocketException ex) { _logger.Add(ex.Message); }
            catch (ObjectDisposedException ex) { _logger.Add(ex.Message); }
           
            if (byteData > 0)
            {
                byte[] data = new byte[byteData];
                Array.Copy(connection.Buffer, data, byteData);

                connection.Data.Put(data);
                _logger.Add(data.Length + " bytes received");

                //ожидание получения очередной порции данных
                Receive(connection);
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            try
            {
                Socket handler = (Socket) result.AsyncState;
                int byteSent = handler.EndSend(result);
                _logger.Add(byteSent.ToString() + " bytes sent to " +
                            handler.RemoteEndPoint.ToString());
            }
            catch (SocketException ex)
            {
                _logger.Add(ex.Message);
            }
        }

        public ConnectionPool Connections
        {
            get { return _connections; }
        }
    }
}
