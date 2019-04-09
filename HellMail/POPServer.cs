using System;

namespace HellMail {

    public class POPServer : TCPServer {

        public POPServer(string IP, int portNumber, string cert) : base(IP, portNumber, cert) {
            this.setName("POP3");
        }

    }
}
