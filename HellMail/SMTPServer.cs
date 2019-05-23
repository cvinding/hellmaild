using System;
using System.Net.Security;
using HellMail.Data;

namespace HellMail {

    public class SMTPServer : TCPServer {

        private readonly string password;

        private bool authenticated = false;

        public SMTPServer(string IP, int portNumber, string cert, string password) : base(IP, portNumber, cert) {
            this.setName("SMTP");
            this.password = password;
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

                if(input.Length >= 11 && input.Substring(0, 10) == "AUTH PLAIN") {
                    input = input.Replace(Environment.NewLine, "");

                    string passwordInput;

                    try {
                        passwordInput = input.Split(" ")[2];

                    } catch (Exception ex) {
                        StreamWrite(client, "-501 Syntax error, usage: AUTH PLAIN <password> ");
                        continue;
                    }

                    if (passwordInput == password) {
                        authenticated = true;
                        StreamWrite(client, "235 Authentication successful");
                    } else {
                        authenticated = false;
                        StreamWrite(client, "501 Syntax error, authentication failed");
                    }
                    continue;
                }

                if (input.Length >= 8 && input.Substring(0, 7) == "RCPT TO" && authenticated) {
                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (input.Length >= 10 && input.Substring(0, 9) == "MAIL FROM" && authenticated) {
                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (command == "DATA" && authenticated) {
                    StreamWrite(client, "354 Start mail input; end with '.\n'");

                    input = StreamRead(client);

                    try {

                        Mail mail = new Mail();

                        mail.Parse(input);

                        Database.InsertMail(mail);

                    } catch (Exception ex) {
                        Logger.Log(ex.ToString());
                        StreamWrite(client, "501 Syntax error");
                        continue;
                    }

                    StreamWrite(client, "250 OK");
                    continue;
                }

                if (authenticated) {
                    StreamWrite(client, "500 unknown command");
                } else {
                    StreamWrite(client, "530 unknown command OR authentication missing");
                }

            }
        }

    }
}
