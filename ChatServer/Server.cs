using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace ChatServer
{
    class Server
    {
        static void Main(string[] args)
        {
            try
            {
                // List of threads to serve multiple clients
                List<Thread> threads = new List<Thread>();

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
    }

    class ClientHandle
    {
        private Socket s;

        public ClientHandle(Socket s) => this.s = s;

        public void Start()
        {
            Thread thread = new Thread(Communicate);
            thread.Start();
        }

        private void Communicate()
        {
            Console.WriteLine("A client connected.");

            // Receive messages from client
            byte[] bytes = new byte[256];
            while (true)
            {
                try
                {
                    int n = s.Receive(bytes);
                    for (int i = 0; i < n; i++)
                    {
                        Console.Write(Convert.ToChar(bytes[i]));
                    }
                    Console.WriteLine();
                }
                catch (SocketException)
                {
                    Console.WriteLine("A client disconnected.");
                    break;
                }
            }
        }
    }
}
