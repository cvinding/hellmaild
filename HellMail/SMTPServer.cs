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
            string command = String.Empty;

            while (true) {

                try {
                    input = StreamRead(client);

                    if (input == "") {
                        client.Close();
                        break;
                    } else {
                        input = input.TrimEnd('\n');
                    }

                    command = input.Split(" ")[0];

                } catch (Exception e) {
                    break;
                }

                if (command == "QUIT") {
                    StreamWrite(client, "221 Goodbye");
                    client.Close();
                    break;//exit while
                }

                //message has successfully been received
                if (command == "EHLO") {
                    StreamWrite(client, "250 " + Helper.GetFQDN() + ", I am glad to meet you");
                    continue;
                }

                if (input.Substring(0, 6) == "RCPT TO") {
                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (input.Substring(0, 8) == "MAIL FROM") {
                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (command == "DATA") {
                    StreamWrite(client, "354 Start mail input; end with '.\n'");

                    input = StreamRead(client);

                    try {

                        Mail mail = new Mail();

                        mail.Parse(input);

                        Database.InsertMail(mail);

                    } catch (Exception) {
                        StreamWrite(client, "501 Syntax error");
                        continue;
                    }

                    StreamWrite(client, "250 OK");
                    continue;
                }


                StreamWrite(client, "500 unknown command");

            }
        }

    }
}
