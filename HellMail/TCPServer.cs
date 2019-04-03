using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HellMail {

    public class TCPServer {

        private string name = "TCP";

        private readonly TcpListener listener;

        public TCPServer(string IP, int portNumber) {
            listener = new TcpListener(IPAddress.Parse(IP), portNumber);
        }

        public void Start() {
            // Start listening 
            listener.Start();
            Console.WriteLine(name + " Server started");

            while (true) {
                // Wait for a client to connect
                Socket client = listener.AcceptSocket();
                Console.WriteLine(name + ": Client connection accepted");

                // Start ProcessClient() on a new Thread
                var clientThread = new Thread(() => ProcessClient(client));
                clientThread.Start();
            }

        }

        protected virtual void ProcessClient(Socket client) { }

        protected void setName(string name) {
            this.name = name;
        }

        // Write() is used to write an message to the client
        protected void Write(Socket client, string message) {
            byte[] msg = Encoding.ASCII.GetBytes(message + "\n");
            client.Send(msg);
        }

        // Read() is used to read the messages from the client
        protected string Read(Socket client) {

            /*int currentByteSize = -1;
            byte[] bytes = new byte[8000];
            string data = null;
            bool blocking = false;*/
            /*int k = s.Receive(b);
            string szReceived = Encoding.ASCII.GetString(data, 0, k);*/

            //client.ReceiveTimeout = 1500;

            string str;
            using (NetworkStream stream = new NetworkStream(client))
            {
                byte[] data = new byte[1024];
                using (MemoryStream ms = new MemoryStream())
                {

                    int numBytesRead;
                    while ((numBytesRead = stream.Read(data, 0, data.Length)) != 0)
                    {
                        ms.Write(data, 0, numBytesRead);


                    }
                    str = Encoding.ASCII.GetString(ms.ToArray(), 0, (int)ms.Length);
                }
            }

            /*while (true) {

                int numByte = client.Receive(bytes);
                currentByteSize = numByte;

                Thread.Sleep(30);

                blocking = client.Blocking;

                data += Encoding.ASCII.GetString(bytes, 0, numByte);

                //Console.WriteLine(blocking);
                //Console.WriteLine(currentByteSize);

                if (blocking == true && numByte == 1) {
                    break;
                }


                //if (data.IndexOf("\n.\n", StringComparison.CurrentCulture) != -1) {
                //    break;
                //}
            }*/

            return str;

        }

    }
}
