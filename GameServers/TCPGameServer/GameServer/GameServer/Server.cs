using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;



//NOTE: Most of the code is similar to the UDP server, for this reason there will be not as much comments in this file

namespace GameServer
{
    class Server //TCP SERVER 
    {
        
        // Inicialization of Class data

        int recv; //Byte counter, to select position into buffer data a
        byte[] data; //buffer data
         static IPEndPoint ipep;//IP
         Socket Oursocket; //Our Socket
         Thread listener =null;//Threat Initialization


        int BacklogClientQueue; //int determining the max number of queue connections
        int PingPongIteration; //How many time the ping pong process is to be repited

      
        public void Start()
        {
           
            data = new byte[1024];//Allocate buffer size
            ipep = new IPEndPoint(IPAddress.Any,9050);//Define  IP and port 
            Oursocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);//stablish Socket and protocol
            Oursocket.Bind(ipep);//Bind socket with IP
            BacklogClientQueue = 5;
            Oursocket.Listen(BacklogClientQueue);//define Queue Backlog
            PingPongIteration = 5;//Times the client and server will send Ping/Pong communication

            Console.WriteLine("Waiting for a client...");

            //BEGIN THREAT-------------------------------------------------------------------------------------------------------
            if (listener==null)
            {
                listener = new Thread(ListenToConnections);
                listener.Start();
            }
            

        }


        void ListenToConnections()
        {
            try
            {
                for (int i = 0; i < BacklogClientQueue; i++)
                {

                    Socket client = Oursocket.Accept(); //Accept stablishes the next client as connection in the queue 
                    IPEndPoint clientep = (IPEndPoint)client.RemoteEndPoint;    //NOTE:Socket.RemoteEndPoints return a object Endpoint with the IP and port number

                    Console.WriteLine("Connected with {0} at port {1}", clientep.Address, clientep.Port);

                    String welcome = "Welcome to my server";
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
                    client.Shutdown(SocketShutdown.Both);//I Shut down (restricts sending and recieving data) the socket due to I know that the program will finish if not then, whe should keep it open for further connections 
                    client.Close();


                }
           
            
            }
            catch
            {
                Console.WriteLine("Thread interrumpted closing connection");

            }
            
             Oursocket.Close();

        }

       public bool GetThreadStatus()
        {


            return listener.IsAlive;
        }

        public void setThreatNull() //set threat to null
        {

            listener = null;

        }


    }
}
