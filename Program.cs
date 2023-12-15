using System;
using System.Net;
using System.IO;
using System.Text;
using System.Threading;

namespace test
{

    class Program
    {
        static void Main(string[] args)
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://127.0.0.1:8888/");
            server.Prefixes.Add("http://localhost:8888/");

            server.Start();

            Console.WriteLine("Listening...");

            while (true)
            {
                Console.WriteLine("The main thread - new request");
                HttpListenerContext context = server.GetContext();

                Console.WriteLine("Main thread start - backgroud work");
                Thread thread = new Thread(new ParameterizedThreadStart(BackgroundWork));
                thread.Start(context); 
                HttpListenerResponse response = context.Response;

               
               
            }
            static void BackgroundWork(Object obj)
            {
                long timeTakenbgw = DateTime.Now.Ticks;
                Console.WriteLine("BG work ID : {0}, started.", timeTakenbgw);
                HttpListenerContext newThread = (HttpListenerContext)obj;
                HttpListenerResponse response = newThread.Response;

                // in this example there is a index.html file in the (root) project folder. 
                string page = @"../../index.html";

                TextReader tr = new StreamReader(page);
                string msg = tr.ReadToEnd();

                byte[] buffer = Encoding.UTF8.GetBytes(msg);

                response.ContentLength64 = buffer.Length;
                Stream st = response.OutputStream;
                st.Write(buffer, 0, buffer.Length);

                newThread.Response.Close();

                Console.WriteLine("BG work ID : {0}, HTTP part done.", timeTakenbgw);
               
                Thread.Sleep(5000);
                Console.WriteLine("BG work ID : {0},end.", timeTakenbgw);
            }

        }
    }

}