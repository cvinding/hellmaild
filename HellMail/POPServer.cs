using System;
using System.Net;
using System.Net.Security;

namespace HellMail {

    public class POPServer : TCPServer {

        private string username;
        private string password;
        private bool authenticated = false;

        public POPServer(string IP, int portNumber, string cert) : base(IP, portNumber, cert) {
            this.setName("HPOP");
        }

        protected override void ProcessClient(SslStream client) {

            StreamWrite(client, "+OK HPOP server ready <" + Helper.GetFQDN() + ">");

            string input = String.Empty;
            while (true) {

                try {
                    input = StreamRead(client);

                } catch (Exception e) {
                    Console.WriteLine(e);
                    break;
                }

                if (input.StartsWith("UD", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "+OK hellmaild HPOP server signing off (maildrop empty)");
                    client.Close();
                    break;//exit while
                }

                // Authenticate the user
                if(input.StartsWith("APOP", StringComparison.CurrentCultureIgnoreCase)) {
                    input = input.Replace(Environment.NewLine, "");

                    string usernameInput;
                    string passwordInput;

                    try {

                        usernameInput = input.Split(" ")[1];
                        passwordInput = input.Split(" ")[2];

                    } catch (Exception ex) {
                        StreamWrite(client, "-ERR Usage: APOP <email> <password> ");
                        continue;
                    }

                    if(usernameInput == "tubbe@hellmail.dk" && passwordInput == "1234") {
                        authenticated = true;

                        username = usernameInput;
                        password = passwordInput;

                        StreamWrite(client, "+OK maildrop locked and ready");
                    } else {
                        authenticated = false;
                        StreamWrite(client, "-ERR permission denied");
                    }
                    continue;
                }

                if (input.StartsWith("STAT", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    StreamWrite(client, "+OK {n0} {n1} " + username);
                    continue;
                }

                if (input.StartsWith("LIST", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    StreamWrite(client, "+OK {n0} messages {n1 octets}");
                    StreamWrite(client, "1 {n1.5}");
                    StreamWrite(client, "2 {n1.5}");
                    StreamWrite(client, ".");
                    continue;
                }

                if (input.StartsWith("RETR", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    StreamWrite(client, "+OK {n1.5} octets");
                    StreamWrite(client, "<the POP3 server sends message>");
                    StreamWrite(client, ".");
                    continue;
                }

                if (input.StartsWith("DELE", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    StreamWrite(client, "+OK message {n2} deleted");
                    continue;
                }

                if(authenticated) {
                    StreamWrite(client, "-ERR Unrecognisable command");
                } else {
                    StreamWrite(client, "-ERR Missing authentication OR unrecognisable command");
                }

            }

        }

    }

}
