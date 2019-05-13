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

                    string type = String.Empty;

                    try {

                        string[] args = input.Split(" ");

                        type = args[1].TrimEnd('\n');

                        if (type != "SENT" && type != "RECIEVED") {
                            StreamWrite(client, "-ERR usage: STAT (SENT/RECIEVED)");
                            continue;
                        }

                    } catch (Exception ex) {
                        StreamWrite(client, "-ERR usage: STAT (SENT/RECIEVED)");
                        continue;
                    }

                    List<int> mailIDs = Database.GetMailIDList(username, type);

                    StreamWrite(client, "+OK " + mailIDs.Count + " messages\n+DONE");
                    continue;
                }

                if (input.StartsWith("LIST", StringComparison.CurrentCultureIgnoreCase) && authenticated) {

                    string type = String.Empty;
                    int startIndex = -1;
                    int endIndex = -1;

                    try {

                        string[] args = input.Split(" ");

                        type = args[1].TrimEnd('\n');

                        if(type != "SENT" && type != "RECIEVED") {
                            StreamWrite(client, "-ERR usage: LIST (SENT/RECIEVED) {[START INDEX], [END INDEX]}");
                            continue;
                        }

                        if (args.Length > 2) {

                            if(!int.TryParse(args[2], out startIndex) || !int.TryParse(args[3], out endIndex)) {
                                StreamWrite(client, "-ERR set index was not a valid number");
                                continue;
                            }

                            if(startIndex <= -1 || endIndex <= -1) {
                                StreamWrite(client, "-ERR indices can not have a negative value");
                                continue;
                            }

                            if (startIndex > endIndex) {
                                StreamWrite(client, "-ERR start index can not be higher than end index");
                                continue;
                            }

                        }

                    } catch (Exception ex) {
                        StreamWrite(client, "-ERR usage: LIST (SENT/RECIEVED) {[START INDEX], [END INDEX]}");
                        continue;
                    }

                    List<int> mailIDs;

                    if (startIndex != -1 && endIndex != -1) {
                        mailIDs = Database.GetMailIDList(username, type, startIndex, endIndex);
                    } else {
                        mailIDs = Database.GetMailIDList(username, type);
                    }

                    StreamWrite(client, "+OK " + mailIDs.Count + " messages");

                    if (mailIDs.Count != 0) {
                        foreach (int id in mailIDs){
                            StreamWrite(client, id + " ");
                        }
                    }

                    StreamWrite(client, "+DONE");
                    continue;
                }

                if (input.StartsWith("HENT", StringComparison.CurrentCultureIgnoreCase) && authenticated) {
                    List<DbMail> dbMails;

                    try {

                        List<string> inputIds = input.Substring(5).TrimEnd('\n').Split(" ").ToList<string>();

                        bool invalid = false;
                        for(int i = 0; i < inputIds.Count; i++) {

                            int tmp;
                            if(!int.TryParse(inputIds[i], out tmp)) {
                                invalid = true;
                                break;
                            }

                        }

                        if(invalid) {
                            StreamWrite(client, "-ERR invalid argument: expected arguments to be numbers");
                            continue;
                        }

                        dbMails = Database.GetMails(username, inputIds);

                    } catch (Exception ex) {
                        StreamWrite(client, "-ERR Usage: RETR <id> <id> ");
                        continue;
                    }

                    string output = "+OK \n"; 

                    if(dbMails.Count > 0) {
                        foreach (DbMail dbMail in dbMails) {
                            output += dbMail.FormatMail();
                        }
                    } else {
                        output += "[INFO] Messages were unretrievable \n";
                    }

                   
                    output += "+DONE";

                    StreamWrite(client, output);

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
