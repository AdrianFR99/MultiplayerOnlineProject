using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;


namespace GameServer
{
    class Server
    {

        int recv;
        byte[] data;
        IPEndPoint ipep; 
        Socket Oursocket;
        Thread listener=null;


        int BacklogClientQueue;
        int PingPongIteration;
      
        public void Start()
        {
           
            data = new byte[1024];
            ipep = new IPEndPoint(IPAddress.Any,9050);
            Oursocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
            Oursocket.Bind(ipep);
            BacklogClientQueue = 5;
            Oursocket.Listen(BacklogClientQueue);
            PingPongIteration = 5;//Times the client and server will send Ping/Pong communication

            Console.WriteLine("Waiting for a client...");

            //BEGIN THREAT-------------------------------------------------------------------------------------------------------
            if (listener==null)
            {
                listener = new Thread(ListenToConnections);
                listener.Start();
            }
              //Oursocket.Close();

        }


        void ListenToConnections()
        {
            try
            {
                for (int i = 0; i < BacklogClientQueue; i++)
                {

                    Socket client = Oursocket.Accept(); //the new socket cannot be use again to Accept the next queue connection 
                    IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;//Socket.RemoteEndPoint return a object Endpoint with the IP and port number

                    Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);

                    String welcome = "Welcome to hell again";
                    data = Encoding.ASCII.GetBytes(welcome);
                    client.Send(data, data.Length, SocketFlags.None);//SEND WELCOME MESSAGE


                    int z = 0;

                    while (z < PingPongIteration)
                    {
                        data = new byte[1024];
                        recv = client.Receive(data);//blocks
                        if (recv == 0)
                        {//if client send 0 byte lenght data then disconnect  

                            break;

                        }
                        Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));

                        client.Send(Encoding.ASCII.GetBytes("Pong"), SocketFlags.None);
                        z++;
                        Thread.Sleep(1000);

                    }

                    Console.WriteLine("Disconnected from {0}",
                                clientep.Address);
                    client.Close();


                }
            Oursocket.Close();
            
            }
            catch
            {
                Console.WriteLine("Thread interrumpted closing connection");
                Oursocket.Close();

            }


        }

       public bool GetThreadStatus()
        {


            return listener.IsAlive;
        }


    }
}
