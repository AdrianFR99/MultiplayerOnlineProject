using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server : MonoBehaviour
{

    int recv;
    byte[] data;
    EndPoint Remote;
   static  Socket newsocket;
    static IPEndPoint ipep;
  
    void Start()
    {
        data = new byte[1024];//allocate buffer size
        ipep = new IPEndPoint(IPAddress.Any,9050);//define Server IP and port 
        newsocket = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);// stablish socket 
        newsocket.Bind(ipep);//Bind socket with IP
        
        Debug.Log("Waiting for a client");

        IPEndPoint sender = new IPEndPoint(IPAddress.Any,0);
        Remote = (EndPoint)(sender);//declaration of an Endpoint var to assign to remote client

        recv = newsocket.ReceiveFrom(data,ref Remote);//waiting to recieve from client, ReceiveFrom(allocates the bytes in memory and assigns recv to the byte count, plus stores remote endpoint)
        //IMPORTANT ReceiveFrom WILL STOP CODE UNTIL SOMETHING IS RECEIVE.

        Debug.Log("Message recieve form:" + Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data,0,recv)); //"convert" message to string and display it

        data = Encoding.ASCII.GetBytes("Welcome To Hell");
        newsocket.SendTo(data,data.Length,SocketFlags.None,Remote);//Send data var to client 
        //IMPORTANT SendTo WILL STOP CODE UNITL MESSAGE IS SENT.

    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}




}
