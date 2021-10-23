using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;


using System.Threading;

public class Client : MonoBehaviour
{

    // Data declaration like in the server
    int recv;
    byte[] data;
    String input, stringData;
    IPEndPoint ipep;
    Socket server;
    EndPoint Remote;

    Thread listener = null;
    bool kill = false;


    // Start is called before the first frame update
    void Start()
    {
        //Data initialization 
        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"),9050);//Defining local IP

        server = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);//defining socket and protocol 

       
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
         Remote = (EndPoint)sender;



    }

    private void Update()
    {

        if (listener!=null && !listener.IsAlive)
        {
           // Debug.Log("thread Alive:" + listener.IsAlive);
            listener = null;
            Debug.Log("Thread closed");
      
          
        }




    }


  public void StartThread()
    {

        if (listener == null)
        {

            listener = new Thread(Connect);
            listener.Start();

        }

    }


    void Connect() {


        try
        {

            String welcome = "Hello???";
            data = Encoding.ASCII.GetBytes(welcome);//encode initial message
            server.SendTo(data, data.Length, SocketFlags.None, ipep);

            data = new byte[1024];
            int recv = server.ReceiveFrom(data, ref Remote);

            Debug.Log("Message recieve form:" + Remote.ToString());
            Debug.Log(Encoding.ASCII.GetString(data, 0, recv));

            int i = 0;

            while (!kill)
            {

                server.SendTo(Encoding.ASCII.GetBytes("Ping"), Remote);
                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref Remote);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                Debug.Log(stringData);
                i++;
                Thread.Sleep(1000);

                if (i == 5 )
                {
                 
                    RequestKillThread();

                }

            }

           
        }
        catch (ThreadInterruptedException exception)
        {

            Debug.Log("thread Interrupted");
            Debug.Log(exception);
            RequestKillThread();


        }

        server.Shutdown(SocketShutdown.Both);//This ensures that all data is sent and received on the connected socket before it is closed.
        server.Close();

    }

    void RequestKillThread()
    {
        kill = true;

    }
 
}
