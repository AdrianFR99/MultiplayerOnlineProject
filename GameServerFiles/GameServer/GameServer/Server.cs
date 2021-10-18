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
        EndPoint Remote;
        static Socket newsocket;
        static IPEndPoint ipep;

        Thread listener = null;


        public void Start()
        {
            data = new byte[1024];//allocate buffer size
            ipep = new IPEndPoint(IPAddress.Any, 9050);//define Server IP and port 
            newsocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);// stablish socket 
            newsocket.Bind(ipep);//Bind socket with IP

            Console.Write("Waiting for a client");

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            Remote = (EndPoint)(sender);//declaration of an Endpoint var to assign to remote client

            ///----From here we should start the thread


            if (listener == null)
            {

                listener = new Thread(listenForMessages);
                listener.Start();


            }
        
        }



        void listenForMessages()
        {


            recv = newsocket.ReceiveFrom(data, ref Remote);//waiting to recieve from client, ReceiveFrom(allocates the bytes in memory and assigns recv to the byte count, plus stores remote endpoint)
                                                           //IMPORTANT ReceiveFrom WILL STOP CODE UNTIL SOMETHING IS RECEIVE.

            Console.Write("Message recieve form:" + Remote.ToString());
            Console.Write(Encoding.ASCII.GetString(data, 0, recv)); //"convert" message to string and display it

            data = Encoding.ASCII.GetBytes("Welcome To Hell");
            newsocket.SendTo(data, data.Length, SocketFlags.None, Remote);//Send data var to client 
                                                                          //IMPORTANT SendTo WILL STOP CODE UNITL MESSAGE IS SENT.



            while (true)
            {

             data = new byte[1024];
            recv = newsocket.ReceiveFrom(data, ref Remote);
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
           
            newsocket.SendTo(Encoding.ASCII.GetBytes("Pong"),Remote);

            }


          //  listener.Abort();

        }       

       


    }
}
