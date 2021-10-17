using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Text;


public class Client : MonoBehaviour
{

    int recv;
    byte[] data;
    String input, stringData;
    IPEndPoint ipep;
    Socket server;

    // Start is called before the first frame update
    void Start()
    {

        data = new byte[1024];
        ipep = new IPEndPoint(IPAddress.Parse("127.0.0.1"),9050);//Defining local IP

        server = new Socket(AddressFamily.InterNetwork,SocketType.Dgram,ProtocolType.Udp);//defining socket and protocol 

        String welcome = "hello???";
        data = Encoding.ASCII.GetBytes(welcome);//encode initial message
        server.SendTo(data,data.Length,SocketFlags.None,ipep);

        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        EndPoint Remote = (EndPoint)sender;

        data = new byte[1024];
        int recv = server.ReceiveFrom(data, ref Remote);

        Debug.Log("Message recieve form:" + Remote.ToString());
        Debug.Log(Encoding.ASCII.GetString(data, 0, recv));


    }




    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
