using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Net.Sockets;

using System.Threading;
using System.IO;

using System.Xml.Serialization;



//NOTE: Most of the code is similar to the UDP server, for this reason there will be not as much comments in this file

namespace GameServer
{
    class Server //TCP SERVER 
    {
        private List<ServerClient> clients;
        private List<ServerClient> disconnectList;

        public int port = 9050; // Port 
        private TcpListener server; // Tcp Listener listens for incoming connection to the server
        private bool serverStarted;

        public void startServer()
        {

            clients = new List<ServerClient>();
            disconnectList = new List<ServerClient>();

            try
            {

                server = new TcpListener(IPAddress.Any,port);
                server.Start(); //The Start method initializes the underlying Socket, binds it to a local endpoint, and listens for incoming connection attempts https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.start?view=net-5.0

                StartListening(); //Start async operations to accept connections //Does NOT block until a client connection is received
                serverStarted = true; //set bool 

                Console.WriteLine("Server Started Succesfully"+"On port:"+port.ToString());
            }
            catch (Exception e)
            {

                Console.WriteLine("Error startign server:"+ e.Message); //Print if error is catched

            }

        }


        //this update funtion should be going in a thread
        public void update()
        {

            if (!serverStarted)
                return;

            for(int i=0;i<clients.Count;i++)//lets check for incomming messages for each serverclient 
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
                        var message = new ClientMessage();                        
                        message = DeserializeMessage(stream);

                        Console.WriteLine(message.messageContent.ToString());
                            //Process recieved data 
                        OnIncomingData(clients[i], message);
                       
                    }

                }

            }

           for(int i=0; i < disconnectList.Count - 1; i++){

                Broadcast(disconnectList[i].clientName + "has disconnected",clients);

                clients.Remove(disconnectList[i]);
                disconnectList.RemoveAt(i);

            }

        }

        private void OnIncomingData(ServerClient c, ClientMessage message)
        {
            //  Console.WriteLine(c.clientName + "has sent the following message:" + data);

            if (message.messageContent.Contains("&NAME"))
            {

                c.clientName = message.messageContent.Split('|')[1];
                Broadcast(c.clientName + " has connected" , clients);
                return;
            }
            Broadcast(c.clientName+":"+ message.messageContent.ToString(), clients);


        }

        private bool Isconnected(TcpClient tcp)  // tip: TcpClient.Client equal to the socket
        {

            try// maybe we are not able to reach a client 
            {

                if(tcp != null && tcp.Client!=null && tcp.Client.Connected)//tcp is null means the TcpClient does not have any data assigned // if tcp.client is different from null then we have a socket connected// and tcp.Client.Connected is true when the client is connected thorugh a remote resource since the last operation 
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
            Broadcast("%NAME", new List<ServerClient>() {clients[clients.Count-1]}); // IMPROVE We should create overload for broadcasting

        }

        private void Broadcast(string data, List<ServerClient> clientL)
        {

            foreach(ServerClient c in clientL)
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
                    Console.WriteLine("Write error :" + e.Message + "To client "+ c.clientName);

                }
            }


        }

        private void OnServerShutDown()
        {

            //should we add list here?
            server.Stop();
            serverStarted = false;
            
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
