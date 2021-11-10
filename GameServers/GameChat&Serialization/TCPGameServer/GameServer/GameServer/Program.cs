using System;
using System.Collections.Generic;
using System.Text;


namespace GameServer
{
    class Program //TCP PROGRAM
    {

       //NEED RO PROPERLY CALL THE FUNCTIONS IN THE SERVER IN ORDER TO THE SERVES CLASSS TO WORK

        static void Main(string[] args)
        {


            Server TestServer = new Server();

            Console.Title = "Game Server";
            TestServer.startServer();

            //while (true)
            //{

            //    if (!TestServer.GetThreadStatus())
            //    {
            //        TestServer.setThreatNull();
            //        break;
            //    }

            //}


            //Console.WriteLine("Server disconnected");
            //Console.WriteLine("Press any key to shut down program");

            //Console.ReadKey();



        }





    }
}
