using System;
using System.Net.Security;

namespace HellMail {

    public class IMAPServer : TCPServer {
    
        public IMAPServer(string IP, int portNumber, string cert) : base(IP, portNumber, cert) {
            this.setName("IMAP");
        }

        protected override void ProcessClient(SslStream client) {

            StreamWrite(client, "* OK IMAP4 Service Ready");

            string input = String.Empty;
            while (true) {

                try {
                    input = StreamRead(client);

                } catch (Exception e) {
                    Console.WriteLine(e);
                    break;
                }

                if (input.StartsWith("LOGOUT", StringComparison.CurrentCultureIgnoreCase)) {
                    StreamWrite(client, "* BYE IMAP4 server terminating connection");
                    client.Close();
                    break;//exit while
                }



            }
        }

    }
}
