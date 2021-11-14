using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Xml.Serialization;

using TMPro;
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

    public void ConnectToServer()
    {

        //if already connected:
        if (socketReady)
            return;

        //Default host / prot values
        string host = "127.0.0.1";
        int port = 9050;

        //Overwrite default host/port values
       // OverwriteDefValues(host, port);


        try
        {

            socket = new TcpClient(host,port);
            stream = socket.GetStream();
            //writer = new StreamWriter(stream);
            //reader = new StreamReader(stream);

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
                TextReader textReader = new StreamReader(stream);
                string longString = string.Empty;
                while (true)

                {
                    string s = string.Empty;
                    try
                    {
                        s = textReader.ReadLine();
                    }
                    catch(Exception e)
                    {
                        Debug.Log("Error reading string:" + e.Message);
                        break;
                    }
                    if (s == null) 
                        break;

                    longString = longString + s;

                    if (s.Contains("clientName")) // name is the last parameter so it is the end of the class (last line to read)
                        break;

                }
                longString = longString + "</ClientMessage>"; //I don't know why the last line pops an error so i'll add it myself
                
                var message = new ClientMessage();
                XmlSerializer serializer = new XmlSerializer(typeof(ClientMessage));
                message = (ClientMessage)serializer.Deserialize(new StringReader(longString));

                OnIncomingData(message);
            }
        }



    }

    private void OnIncomingData(ClientMessage message)
    {
        //Debug.Log("Server :"+data);
        if (message.messageContent.ToString() != null)
        {
            if (message.messageContent.ToString() == "%NAME")
            {

                Send("&NAME|" + clientName);
                return;

            }
        }
       GameObject go = Instantiate(messagePrefab, chatContainer.transform);
        go.GetComponentInChildren<TextMeshProUGUI>().text = message.messageContent.ToString(); // Potential error Textmesh pro is type Text?

    }

    private void Send(string data)
    {

        if (!socketReady)
            return;
        else
        {
            //data passed as an argument to Send()
            //encode data to send to the server
            var message = new ClientMessage();
            message.messageContent = data;
            message.clientName = clientName;

            TextWriter textWriter = new StreamWriter(stream);

            XmlSerializer clientMessageSerializer = new XmlSerializer(typeof(ClientMessage));

            clientMessageSerializer.Serialize(stream, message); //From what i understand, this method serializes the data and uses the stream to send it
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

        //writer.Close();
        //reader.Close();
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

    public class ServerClient // we need this class to store a list of clients, who are those which are connected 
    {
        public TcpClient tcp; //socket assignation 
        public string clientName;// client name

        public ServerClient(TcpClient clientSocket)//contructor where we define the client name and the tcp socket 
        {
            clientName = "Guest";
            tcp = clientSocket;
        }
    }

    public struct ClientMessage // struct to serialize when sending a message. 
    {
        public string messageContent;
        public string clientName;
        //color
    }
}
