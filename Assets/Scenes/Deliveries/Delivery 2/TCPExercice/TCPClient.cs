using System;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;

public class TCPClient : MonoBehaviour
{

    byte[] data;
    string input, stringData;
    IPEndPoint ipep;
    Socket server;
    int recv;

    Thread listener;

    void Start()
    {

      data = new byte[1024];
      ipep =  new IPEndPoint(IPAddress.Parse("127.0.0.1"),9050);
      server = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
      listener = null;
      
        try
        {

            server.Connect(ipep);//establishes a network connection between LocalEndPoint and the specified remote endpoint
            Debug.Log("Connected with server");
            listener = new Thread(ListenToMessages);
            listener.Start();

        }
        catch(SocketException e)
        {

            Debug.Log("Unable to connect to server.");
            Debug.Log(e.ToString());
            return;
        }

    
    }

   
    void Update()
    {
        
    }

    void ListenToMessages()
    {

        recv = server.Receive(data);
        stringData = Encoding.ASCII.GetString(data, 0, recv);
        Debug.Log(stringData);

        int i = 0;
        while (true)
        {

            if (i >= 5)
            {
                break;
            }

            server.Send(Encoding.ASCII.GetBytes("Ping"));
            data = new byte[1024];
            recv = server.Receive(data);
            stringData = Encoding.ASCII.GetString(data, 0, recv);
            Debug.Log(stringData);
            i++;
            Thread.Sleep(1000);
        }

        Console.WriteLine("Disconnecting from server...");
        server.Shutdown(SocketShutdown.Both); //This ensures that all data is sent and received on the connected socket before it is closed.
        server.Close();



    }

}
