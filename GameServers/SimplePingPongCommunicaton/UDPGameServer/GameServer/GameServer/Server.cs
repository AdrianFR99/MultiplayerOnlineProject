using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;


namespace GameServer
{
    class Server //UDP SERVER 
    {


        // Inicialization of Class data

        int recv; //Byte counter, to select position into buffer data array
        byte[] data; //buffer data
        EndPoint Remote; // Remote EndPoint Connection 
        Socket newsocket; //Our Socket
        IPEndPoint ipep;//IP

         Thread listener = null; //Threat Initialization
         bool kill = false;//This boolean will be use to exit the Thread properly

        public void Start()
        {
            data = new byte[1024]; //Allocate buffer size
            ipep = new IPEndPoint(IPAddress.Any, 9050); //Define  IP and port 
            newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //stablish Socket and protocol
            newsocket.Bind(ipep);//Bind socket with IP

            Console.WriteLine("Waiting for a client");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)(sender); //Declaration of an Endpoint var to assign to remote client

          
            //Start Threat 

            if (listener == null)
            {
                listener = new Thread(listenForMessages);
                listener.Start();

            }
        
        }



        void listenForMessages()
        {

            try
            {

                recv = newsocket.ReceiveFrom(data, ref Remote);//waiting to recieve from client, ReceiveFrom(allocates the bytes in memory and assigns recv to the byte count, plus stores remote endpoint)
                                                               //IMPORTANT ReceiveFrom WILL STOP CODE UNTIL SOMETHING IS RECEIVE.
                Console.WriteLine("Message recieve form:" + Remote.ToString());
                Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv)); //"convert" message to string and display it

                data = Encoding.ASCII.GetBytes("Welcome To My server");
                newsocket.SendTo(data, data.Length, SocketFlags.None, Remote);//Send data var to client 
                                                                              //IMPORTANT SendTo WILL STOP CODE UNITL MESSAGE IS SENT.

                int i = 0;

                while (!kill) // start Ping Pong Process 
                {
                    data = new byte[1024];
                    recv = newsocket.ReceiveFrom(data, ref Remote);
                    Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
                    newsocket.SendTo(Encoding.ASCII.GetBytes("Pong"), Remote);
                    i++;

                    Thread.Sleep(1000); //I make the threat wait for CMD readability

                    if (i == 5)
                    {
                        RequestKillThread();
                        break;
                    }
                }

            }
            catch(ThreadInterruptedException exception)
            {

                Console.WriteLine("Error Thread Interrumpted");
                Console.WriteLine(exception);
                RequestKillThread();

            }

            newsocket.Shutdown(SocketShutdown.Both); //I Shut down (restricts sending and recieving data) the socket due to I know that the program will finish if not then, whe should keep it open for further connections 
            newsocket.Close(); //Close sockets
            
        }

        void RequestKillThread() // Change Boolean value
        {
            kill = true;

        }


       public bool GetThreatStatus()//Get function to return boolean value
        {
            return listener.IsAlive;
        }

        public void setThreatNull() //set threat to null
        {

            listener = null;

        }

    }
}
