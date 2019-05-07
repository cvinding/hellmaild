using System;
using System.Net;
using System.Net.Security;
using System.Linq;
using System.Linq.Expressions;
using HellMail.Data;
using HellMail.Domain;
using System.Collections.Generic;


namespace HellMail {

    public class POPServer : TCPServer {

        private string username;
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

                    HellMailContext context = new HellMailContext();

                    if(context.Users.Any(u => u.email == usernameInput) && passwordInput == "1234") {
                        authenticated = true;

                        username = usernameInput;

                        StreamWrite(client, "+OK maildrop locked and ready");
                    } else {
                        authenticated = false;
                        StreamWrite(client, "-ERR permission denied");
                    }
                    continue;
                }

                if (input.StartsWith("STAT", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    List<int> mailIDs = Database.GetMailIDList(username);

                    StreamWrite(client, "+OK " + mailIDs.Max());
                    continue;
                }

                if (input.StartsWith("LIST", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    List<int> mailIDs = Database.GetMailIDList(username);
                    
                    StreamWrite(client, "+OK " + mailIDs.Max() + " messages");

                    foreach(int id in mailIDs) {
                        StreamWrite(client, id + " ");
                    }

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
