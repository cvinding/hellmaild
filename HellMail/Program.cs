using System;
using System.Net;

namespace HellMail {

    class MainClass {

        public static void Main(string[] args) {

            for(int i = 0; i < args.Length; i++) {
                Console.WriteLine(i + ":" + args[i]);
            }

            try {
                SMTPServer server = new SMTPServer("0.0.0.0", int.Parse(args[1]), args[0]);

                server.Start();

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

        }
    }
}
