using Microsoft.VisualBasic.CompilerServices;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Listen();
            Console.ReadKey();
        }

        private static void Listen()
        {
            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8888/connection/");
                listener.Start();
                Console.WriteLine("Ожидание подключений...");

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();

                    var handler = InitChain();
                    handler.HandlerRequest(context);
                }
            }
        }

        private static StreamHandler InitChain()
        {
            StreamHandler sh = new StreamHandler();
            ConvertHandler ch = new ConvertHandler();

            sh.Successor = ch;

            return sh;
        }
    }
}
