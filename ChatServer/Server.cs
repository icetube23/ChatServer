﻿using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ChatServer
{
    class Server
    {
        public static List<Socket> clientList = new List<Socket>();

        static void Main(string[] args)
        {
            try
            {
                // IP Address and port to start server on
                IPAddress ipAddress = IPAddress.Parse("192.168.179.47");
                int port = 8000;

                // Create listener for server app
                TcpListener server = new TcpListener(ipAddress, port);

                // Display successful start of server application
                server.Start();
                Console.WriteLine("Server running on port: " + port);
                Console.WriteLine("Local end point: " + server.LocalEndpoint);

                // Wait for clients to connect
                while (true)
                {
                    Socket s = server.AcceptSocket();
                    clientList.Add(s);
                    ClientHandle client = new ClientHandle(s);
                    client.Start();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Server application ended with " + e.StackTrace);
                Console.Read();
            }
        }

        public static void Broadcast(string msg, string client, bool flag = true)
        {
            // Create byte array from message to be broadcasted
            byte[] bytes = flag ? Encoding.UTF8.GetBytes(client + ": " + msg)
                                : Encoding.UTF8.GetBytes(msg);

            // Write the message to all available clients
            foreach (Socket s in clientList)
            {
                s.Send(bytes, 0, bytes.Length, SocketFlags.None);
            }
        }
    }

    class ClientHandle
    {
        private Socket s;
        private string name;

        public ClientHandle(Socket s) => this.s = s;

        public void Start()
        {
            // Start a new thread for communication with client
            new Thread(Communicate).Start();
        }

        private void Communicate()
        {
            GetName();
            Console.WriteLine("Client " + name + " connected.");
            Server.Broadcast(name + " joined the server.", name, false);

            // Receive messages from client
            byte[] bytes = new byte[512];
            while (true)
            {
                try
                {
                    // Read and decode incoming bytes
                    int n = s.Receive(bytes);
                    string received = Encoding.UTF8.GetString(bytes).Substring(0, n);
                    Console.WriteLine(name + ": " + received);

                    // Broadcast received message to all clients
                    if (received == "0x3c0x630x6c0x6f0x730x690x6e0x670x200x63" +
                                    "0x6f0x6e0x6e0x650x630x740x690x6f0x6e0x3e")
                    {
                        Close();
                        break;
                    }
                    else if (received != string.Empty)
                    {
                        Server.Broadcast(received, name);
                    }
                }
                catch (SocketException)
                {
                    Close();
                    break;
                }
            }
        }

        private void GetName()
        {
            // Receive client name
            byte[] bytes = new byte[512];
            try
            {
                int n = s.Receive(bytes);
                name = Encoding.UTF8.GetString(bytes).Substring(0, n);
                if (name.Length > 16) { name = name.Substring(0, 16); }
            }
            catch
            {
                // If client name can't be received, close socket
                s.Close();
            }
        }

        private void Close()
        {
            // Close sockets and remove client
            s.Close();
            Server.clientList.Remove(s);
            Console.WriteLine("Client " + name + " disconnected.");
            Server.Broadcast(name + " left the server.", name, false);
        }
    }
}
