using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class ClientScript : MonoBehaviour
{

    public GameObject chatContainer;
    public GameObject messagePrefab;
    public string clientName; 

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;


    public void ConnectToServer()
    {

        //if already connected:
        if (socketReady)
            return;

        //Default host / prot values
        string host = "127.0.0.1";
        int port = 6321;

        //Overwrite default host/port values
        OverwriteDefValues(host, port);


        try
        {

            socket = new TcpClient(host,port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true; 

        }
        catch(Exception e)
        {

            Debug.Log("SocketError:"+e.Message);

        }
    }


    private void Update()
    {

        if (socketReady)
        {


            if (stream.DataAvailable)
            {


                string data = reader.ReadLine();
                if(data != null)
                {

                    OnIncomingData(data);

                }

            }


        }



    }

    private void OnIncomingData(string data)
    {


        //Debug.Log("Server :"+data);

        if (data == "%NAME") {

            Send("&NAME|"+ clientName);
            return;

                }
       GameObject go = Instantiate(messagePrefab, chatContainer.transform);
        go.GetComponentInChildren<Text>().text = data; // Potential error Textmesh pro is type Text?

    }


    private void Send(string data)
    {


        if (!socketReady)
            return;
        else
        {
            writer.WriteLine(data);
            writer.Flush();


        }


    }

    public void OnSendButton()
    {
        string message = GameObject.Find("InputBox").GetComponent<InputField>().text;
        Send(message);


    }

    private void OverwriteDefValues(string s,int i)
    {

        string h;
        int p;

        h = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (h != "")
            s = h;
        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        if (p != 0)
           i = p;



    }

    private void CloseSocket()
    {
        if (!socketReady)
            return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;

    }

    private void OnApplicationQuit()
    {


        CloseSocket();
    }
    private void OnDisable()
    {
        CloseSocket();
    }

}
