using System;

namespace HellMail {

    class MainClass {

        public static void Main(string[] args) {

            try {
                SMTPServer server = new SMTPServer("0.0.0.0", 1025);

                server.Start();

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

        }
    }
}
