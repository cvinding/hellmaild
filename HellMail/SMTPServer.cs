using System;
using System.Net.Sockets;

namespace HellMail {

    public class SMTPServer : TCPServer {

        public SMTPServer(string IP, int portNumber) : base(IP, portNumber) {
            this.setName("SMTP");
        }

        protected override void ProcessClient(Socket client) {

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
                    Mail mail = new Mail();

                    mail.Parse(strMessage);

                    Write(client, "250 OK");
                }   
            }
        }

    }
}
