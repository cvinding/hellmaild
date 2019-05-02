using System;
using System.Net.Security;

namespace HellMail {

    public class SMTPServer : TCPServer {

        public SMTPServer(string IP, int portNumber, string cert) : base(IP, portNumber, cert) {
            this.setName("SMTP");
        }

        // ProcessClient is the SMTP server's way of handling SMTP clients
        protected override void ProcessClient(SslStream client) {

            StreamWrite(client,"220 " + Helper.GetFQDN() + " hellmaild");

            string input = String.Empty;
            while (true) {

                try {
                    input = StreamRead(client);

                } catch (Exception e) {
                    Console.WriteLine(e);
                    break;
                }

                if (input.StartsWith("QUIT", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "221 Goodbye");
                    client.Close();
                    break;//exit while
                }

                //message has successfully been received
                if (input.StartsWith("EHLO", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "250 " + Helper.GetFQDN() + ", I am glad to meet you");
                    continue;
                }

                if (input.StartsWith("RCPT TO", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (input.StartsWith("MAIL FROM", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (input.StartsWith("DATA", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "354 Start mail input; end with '.\\n'");

                    input = StreamRead(client);

                    //BRUG måske 501

                    StreamWrite(client, "250 OK");
                    continue;
                }


                StreamWrite(client, "500 unknown command");

            }
        }

    }
}
