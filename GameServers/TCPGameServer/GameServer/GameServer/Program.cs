using System;
using System.Collections.Generic;
using System.Text;


namespace GameServer
{
    class Program
    {

       

        static void Main(string[] args)
        {

            Server TestServer = new Server();

            Console.Title = "Game Server";
            TestServer.Start();

            while (true)
            {

                if (!TestServer.GetThreadStatus())
                {
                    break;
                }
                
            }


            Console.WriteLine("Server disconnected");
            Console.WriteLine("Press any key to shut down program");
            
            Console.ReadKey();

           

        }



        
    
    }
}
