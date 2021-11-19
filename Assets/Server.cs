using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.IO;
using System.Xml.Serialization;

public class Server : MonoBehaviour
{

    private List<ServerClient> clients;
    private List<ServerClient> disconnectList;

    public int port = 9050; // Port 
    private TcpListener server; // Tcp Listener listens for incoming connection to the server
    private bool serverStarted;

    string SMstring = "$SM|";

    private Commands commands;

    // Start is called before the first frame update
    void Start()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();
        try
        {

            server = new TcpListener(IPAddress.Any, port);
            server.Start(); //The Start method initializes the underlying Socket, binds it to a local endpoint, and listens for incoming connection attempts https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.start?view=net-5.0

            StartListening(); //Start async operations to accept connections 
            serverStarted = true; //set bool 

            Console.WriteLine("Server Started Succesfully" + "On port:" + port.ToString());

            commands = new Commands();
        }
        catch (Exception e)
        {

            Console.WriteLine("Error startign server:" + e.Message); //Print if error is catched

        }
    }

    // Update is called once per frame
    void Update() //this update funtion should be going in a thread
    {
        if (!serverStarted)
            return;

        for (int i = 0; i < clients.Count; i++)//lets check for incomming messages for each serverclient 
        {


            if (!Isconnected(clients[i].tcp))// check if the client is connected if not:
            {
                clients[i].tcp.Close();
                disconnectList.Add(clients[i]);
                continue;

            }
            else// if connected then 
            {

                NetworkStream stream = clients[i].tcp.GetStream();
                if (stream.DataAvailable)
                {

                    var message = new ClientMessage();//Created struct to deserialize 
                    message = DeserializeMessage(stream);

                    Console.WriteLine(message.messageContent.ToString());
                    //Process recieved data 
                    OnIncomingData(clients[i], message);

                }



            }

        }

        for (int i = 0; i < disconnectList.Count - 1; i++)
        {

            Broadcast(SMstring + disconnectList[i].clientName + "has disconnected", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);

        }
    }

    private void OnIncomingData(ServerClient c, ClientMessage message)
    {
        //  Console.WriteLine(c.clientName + "has sent the following message:" + data);

        if (message.messageContent != null)
        {

            if (message.messageContent.Contains("&NAME"))
            {

                commands.RegisterNameCM(SMstring, message.messageContent, c, clients);

            }
            else if (message.messageContent.Contains("&HELP"))
            {
                commands.HelpCM(c);
            }
            else if (message.messageContent.Contains("&LIST"))
            {

                commands.ListCM(c, clients);

            }
            else if (message.messageContent.Contains("&WHISPER"))//EXAMPLE---> $WHISPER|Adrian:hello there? 
            {

                commands.WhisperCM(message.messageContent, c, clients);


            }
            else
            {
                Broadcast(c.clientName + ":" + message.messageContent, clients);
            }
        }

    }

    private bool Isconnected(TcpClient tcp)  // tip: TcpClient.Client equal to the socket
    {

        try// maybe we are not able to reach a client 
        {

            if (tcp != null && tcp.Client != null && tcp.Client.Connected)//tcp is null means the TcpClient does not have any data assigned // if tcp.client is different from null then we have a socket connected// and tcp.Client.Connected is true when the client is connected thorugh a remote resource since the last operation 
            {

                if (tcp.Client.Poll(0, SelectMode.SelectRead))//The Poll method checks the state of the Socket. IMPORTANT Poll first paramenter is the time to wait for a respons in MICROSECONDS https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket.poll?view=net-5.0
                {
                    return !(tcp.Client.Receive(new byte[1], SocketFlags.Peek) == 0);   // if the signal sent is not zero (meaning the client wants to disconnect)then send true  

                }
                return true;
            }
            else
            {
                Console.WriteLine("Error when reaching client Socket is null or not connected");
                return false;

            }

        }
        catch
        {
            Console.WriteLine("Client has not been reach");
            return false;
        }

    }

    private void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);//Begins an asynchronous operation to accept an incoming connection attempt.
                                                             //IMPORTANT AcceptTcpClient BLOCKS
    }

    private void AcceptTcpClient(IAsyncResult ar) //AsyncCallback to store clients in lists, 
    {

        TcpListener listener = (TcpListener)ar.AsyncState; //This property returns the object that is the last parameter of the method that initiates an asynchronous operation.

        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));//EndAcceptTcpClient End asynchronous operation 
        StartListening();//once a client has been accepted then we wait for another conncetion 


        // Here we should display a connection in the chat, e.j. Adrián has connected

        //Broadcast(clients[clients.Count-1].clientName + "has connected",clients);
        Broadcast("%NAME", new List<ServerClient>() { clients[clients.Count - 1] }); // IMPROVE We should create overload for broadcasting

    }

    internal void Broadcast(string data, List<ServerClient> clientL)
    {

        foreach (ServerClient c in clientL)
        {

            try
            {
                //get client stream
                NetworkStream stream = c.tcp.GetStream();
                //data passed as an argument to Broadcast()
                //encode data to send to the user
                var message = new ClientMessage();
                message.messageContent = data;
                message.clientName = c.clientName;

                SerializeMessage(stream, message);


            }
            catch (Exception e)
            {
                Console.WriteLine("Write error :" + e.Message + "To client " + c.clientName);

            }
        }


    }
    internal void Broadcast(string data, ServerClient c)
    {



        try
        {
            //get client stream
            NetworkStream stream = c.tcp.GetStream();
            //data passed as an argument to Broadcast()
            //encode data to send to the user
            var message = new ClientMessage();
            message.messageContent = data;
            message.clientName = c.clientName;

            SerializeMessage(stream, message);


        }
        catch (Exception e)
        {
            Console.WriteLine("Write error :" + e.Message + "To client " + c.clientName);

        }



    }


    public void OnServerShutDown()
    {

        //We disconnect each client
        foreach (ServerClient sc in clients)
        {
            sc.tcp.Close();
            disconnectList.Add(sc);

        }
        clients.Clear();

        Console.WriteLine("clients disconnocted from server");

        server.Stop();
        serverStarted = false;
        Console.WriteLine("Server conncetion closed");




    }

    public bool GetServerStatus() { return serverStarted; }


    public void SerializeMessage(Stream stream, ClientMessage message)
    {
        XmlSerializer clientMessageSerializer = new XmlSerializer(typeof(ClientMessage));

        clientMessageSerializer.Serialize(stream, message); //From what i understand, this method serializes the data and uses the stream to send it

    }

    public ClientMessage DeserializeMessage(Stream stream)
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
            catch (Exception e)
            {
                Console.WriteLine("Error reading string:" + e.Message);
                break;
            }
            if (s == null)
                break;

            longString = longString + s;

            if (s.Contains("clientName")) // name is the last parameter so it is the end of the class (last line to read)
                break;

        }
        longString = longString + "</ClientMessage>"; //I don't know why the last line pops an error so i'll add it myself

        XmlSerializer serializer = new XmlSerializer(typeof(ClientMessage));
        return (ClientMessage)serializer.Deserialize(new StringReader(longString));
    }
}




class Commands : Server
{

    public void RegisterNameCM(string SM, string data, ServerClient c, List<ServerClient> list)
    {
        c.clientName = data.Split('|')[1];
        Broadcast(SM + c.clientName + " has connected", list);
        Console.WriteLine("Client Identify as :" + c.clientName);



    }

    public void HelpCM(ServerClient c)
    {

        Broadcast("&LIST To see players connected," +
            "&WHISPER to talk directly to a player,", c);//TODO: write correct message


    }
    public void ListCM(ServerClient c, List<ServerClient> list)
    {
        string aux = null;

        foreach (ServerClient sc in list)
        {

            if (aux == null)
                aux = sc.clientName + "@";
            else
                aux = aux + "@" + sc.clientName + "@";

        }

        aux = aux.Replace("@", System.Environment.NewLine);
        Broadcast(aux, c);
        return;



    }
    public void WhisperCM(string data, ServerClient c, List<ServerClient> list)
    {

        string[] Auxdata = data.Split('|', ':');

        string user = Auxdata[1];
        string message = Auxdata[2];

        foreach (ServerClient sc in list)
        {

            if (sc.clientName == user)
            {

                Broadcast(c.clientName + ":" + message, sc);
                break;
            }
        }


    }
    private void OnDestroy()
    {
        OnServerShutDown();
    }


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