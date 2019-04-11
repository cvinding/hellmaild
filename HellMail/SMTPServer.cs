using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;

namespace HellMail {

    public class SMTPServer : TCPServer {

        public SMTPServer(string IP, int portNumber, string cert) : base(IP, portNumber, cert) {
            this.setName("SMTP");
        }

        protected override void ProcessClient(SslStream client) {

            Write(client,"220 " + Environment.MachineName + " hellmaild");

            string input = String.Empty;
            while (true) {

                try {
                    input = ReadLine(client);

                    Console.Write("input:" + input);

                } catch (Exception e) {
                    Console.WriteLine(e);
                    break;
                }

                if (input.StartsWith("QUIT", StringComparison.CurrentCulture)) {
                    Write(client, "221 Goodbye");
                    client.Close();
                    break;//exit while
                }

                //message has successfully been received
                if (input.StartsWith("EHLO", StringComparison.CurrentCulture)) {
                    Write(client, "250 " + Environment.MachineName + ", I am glad to meet you");
                    continue;
                }

                if (input.StartsWith("RCPT TO", StringComparison.CurrentCulture)) {
                    Write(client, "250 OK");
                    continue;
                }

                if (input.StartsWith("MAIL FROM", StringComparison.CurrentCulture)) {
                    Write(client, "250 OK");
                    continue;
                }

                if (input.StartsWith("DATA", StringComparison.CurrentCulture)) {
                    Write(client, "354 Start mail input; end with '.\\n'");

                    input = ReadLine(client);

                    //BRUG måske 501

                    Write(client, "250 OK");
                    continue;
                }


                Write(client, "500 unknown command");

            }
        }

    }
}
