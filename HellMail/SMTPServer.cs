using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HellMail {

    public class SMTPServer {

        private readonly TcpListener listener;

        public SMTPServer(string IP, int portNumber) {
            listener = new TcpListener(IPAddress.Parse(IP), portNumber);
        }

        public void Start() {

            // Start listening 
            listener.Start();
            Console.WriteLine("Server started");

            while(true) {
                // Wait for a client to connect
                Socket client = listener.AcceptSocket();
                Console.WriteLine("Client connection accepted");

                // Start ProcessClient() on a new Thread
                var clientThread = new Thread(() => ProcessClient(client));
                clientThread.Start();
            }

        }

        private void ProcessClient(Socket client) {

            Write(client,"220 localhost -- Fake proxy server");

            string strMessage = String.Empty;
            while (true) {

                try {
                    strMessage = Read(client);
                } catch (Exception e) {
                    Console.WriteLine(e);
                    break;
                }

                if (strMessage.Length <= 0) {
                    continue;
                }

                if (strMessage.StartsWith("QUIT", StringComparison.CurrentCulture)) {
                    client.Close();
                    break;//exit while
                }

                //message has successfully been received
                if (strMessage.StartsWith("EHLO", StringComparison.CurrentCulture)) {
                    Write(client, "250 OK");
                }

                if (strMessage.StartsWith("RCPT TO", StringComparison.CurrentCulture)) {
                    Write(client, "250 OK");
                }

                if (strMessage.StartsWith("MAIL FROM", StringComparison.CurrentCulture)) {
                    Write(client, "250 OK");
                }

                if (strMessage.StartsWith("DATA", StringComparison.CurrentCulture)) {
                    Write(client, "354 Start mail input; end with");
                    strMessage = Read(client);
                    Write(client, "250 OK");
                }   
            }
        }

        private void Write(Socket client, string message) {
            byte[] msg = Encoding.ASCII.GetBytes(message + "\n");
            client.Send(msg);
        }

        private string Read(Socket client) {
            byte[] data = new byte[8000];
            int size = client.Receive(data);
            return Encoding.ASCII.GetString(data, 0, size);
        }

    }
}
