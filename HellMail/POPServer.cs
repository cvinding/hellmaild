using System;

namespace HellMail {

    public class POPServer : TCPServer {

        public POPServer(string IP, int portNumber) : base(IP, portNumber) {
            this.setName("POP3");
        }

    }
}
