using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace NetworkModules3
{
    public class TcpEchoServerSample
    {
        public const int PORT = 8888;
        public bool isEnd;
        public TcpEchoServerSample()
        {
            isEnd = false;
        }

        public void Echo()
        {
            TcpListener listener = null;
            NetworkStream ns = null;
            StreamReader sr = null;
            StreamWriter sw = null;
            TcpClient client = null;

            try
            {
                IPAddress address = Dns.GetHostEntry("127.0.0.1").AddressList[0];
                listener = new TcpListener(address, PORT);
                listener.Start();
                Console.WriteLine("server ready...");
                while (!isEnd)
                {
                    client = listener.AcceptTcpClient();
                    Console.WriteLine("accept client!");
                    ns = client.GetStream();
                    sr = new StreamReader(ns, Encoding.Default);
                    sw = new StreamWriter(ns, Encoding.Default);
                    while (!isEnd)
                    {
                        string msg = string.Empty;
                        if (String.IsNullOrEmpty(msg = sr.ReadLine()) == false)
                        {
                            Console.WriteLine("server::"+msg);
                            sw.WriteLine(msg);
                        }
                        else
                        {
                            isEnd = true;
                            Console.WriteLine("server end!");
                            break;
                        }
                    }
                }
                Console.WriteLine("exit from while in Server...");

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (sr != null) sr.Close();
                if (client != null) client.Close();
                listener.Stop();
            }
        }
        public void end()
        {
            isEnd = true;
        }

    }
}

            
