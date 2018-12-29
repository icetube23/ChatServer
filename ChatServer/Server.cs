using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Server
    {
        static void Main(string[] args)
        {
            try
            {
                IPAddress ipAddress = IPAddress.Parse("192.168.179.47");
                int port = 8000;

                // Create listener for server app
                TcpListener server = new TcpListener(ipAddress, port);

                // Display successful start of server application
                server.Start();
                Console.WriteLine("Server running on port: " + port);
                Console.WriteLine("Local end point: " + server.LocalEndpoint);

                // Wait for a client to connect
                Socket s = server.AcceptSocket();

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
                        s = server.AcceptSocket();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Server application ended with " + e.StackTrace);
                Console.Read();
            }
            
        }
    }
}
