using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace HellMail {

    public class TCPServer {

        private string name = "TCP";

        private X509Certificate2 serverCertificate = null;

        private readonly TcpListener listener;

        public TCPServer(string IP, int portNumber, string cert) {

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.SystemDefault;
            // Set Self Signed Certificate 
            serverCertificate = new X509Certificate2(cert, "1234");

            listener = new TcpListener(IPAddress.Parse(IP), portNumber);
        }

        public void Start() {
            // Start listening 
            listener.Start();
            Console.WriteLine(name + " Server started");

            while (true) {
                // Wait for a client to connect
                //Socket client = listener.AcceptSocket();
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine(name + ": Client connection accepted");

                // Start ProcessClient() on a new Thread
                var clientThread = new Thread(() => ProcessClient(client));
                clientThread.Start();
            }

        }

        protected virtual void ProcessClient(Socket client) { }

        protected virtual void ProcessClient(TcpClient client) { }

        protected void setName(string name) {
            this.name = name;
        }

        // Write() is used to write an message to the client
        protected void Write(Socket client, string message) {

            NetworkStream networkStream = new NetworkStream(client, false);

            SslStream sslStream = new SslStream(networkStream, false);

            try {
                sslStream.AuthenticateAsServer(serverCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            byte[] msg = Encoding.ASCII.GetBytes(message + "\n");
            sslStream.Write(msg);
        }

        protected void Write(TcpClient client, string message) {

            SslStream sslStream = new SslStream(client.GetStream(), false);

            try {
                sslStream.AuthenticateAsServer(serverCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            byte[] msg = Encoding.ASCII.GetBytes(message + "\n");
            sslStream.Write(msg);
        }

        // Read() is used to read the messages from the client
        /*protected string Read(Socket client) {

            /*NetworkStream networkStream = new NetworkStream(client, false);

            SslStream sslStream = new SslStream(networkStream, false);
            
            try {
                sslStream.AuthenticateAsServer(serverCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }*/

            //8192
           /* byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do {
                // Read the client's test message.
                bytes = client.Receive(buffer);
                Console.WriteLine(messageData.ToString());

                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>", StringComparison.OrdinalIgnoreCase) != -1) {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }*/

        /*protected string Read(TcpClient client) {
            SslStream sslStream = new SslStream(client.GetStream(), false);

            try {
                sslStream.AuthenticateAsServer(serverCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            //8192
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                // Read the client's test message.
                bytes = sslStream.Read(buffer, 0, buffer.Length);


                // Use Decoder class to convert from bytes to UTF8
                // in case a character spans two buffers.
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                // Check for EOF or an empty message.
                if (messageData.ToString().IndexOf("<EOF>", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }*/

        protected string ReadLine(TcpClient client) {

            //NetworkStream networkStream = new NetworkStream(client, false);

            SslStream sslStream = new SslStream(client.GetStream(), false);

            try {
                sslStream.AuthenticateAsServer(serverCertificate, false, System.Security.Authentication.SslProtocols.Tls12, false);
            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            byte[] buffer = new byte[1024];
            int n = sslStream.Read(buffer, 0, buffer.Length);

            string _message = Encoding.UTF8.GetString(buffer, 0, n);

            Console.WriteLine(_message);

            return _message;
        }

        /*protected string ReadMultiLine(Socket client, string end) {
            string currentLine;
            string result = "";

            do {

                currentLine = ReadLine(client);
                result += currentLine;

                Console.Write(currentLine.Length + ": " + currentLine);

            } while (currentLine.Replace(System.Environment.NewLine, "") != end);

            Console.WriteLine("DU KOM UD");

            return result;
        }*/

    }
}
