using System;
using System.Collections.Generic;
using System.Text;


namespace GameServer
{
    class Program //UDP PROGRAM
    {

        static void Main(string[] args)
        {

            Server TestServer = new Server();

            Console.Title = "Game Server";
            TestServer.Start();


            while (true) //While threat is alive
            {

                if (!TestServer.GetThreatStatus())//Else set thread to null and break
                {

                    TestServer.setThreatNull();
                    break;

                }



            }


            Console.WriteLine("Server disconnected");
            Console.WriteLine("Press any key to shut down program");
            Console.ReadKey();
        }



        
    
    }
}
