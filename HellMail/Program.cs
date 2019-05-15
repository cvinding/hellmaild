using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace HellMail {

    class MainClass {

        public static void Main(string[] args) {

            /*string mailData = 
                "From: kent@hellmail.dk\n" +
                "To: christian@hellmail.dk\n" +
                "Cc: tubbe@hellmail.dk\n" +
                "Subject: Møde tider\n" + 
                "\n" +
                "Hej aber \n" +
                "Trump should be a peach \n" +
                "Upkiblers to the left \n" +
                ".\n" +               
                ".\n"
            ;


            Mail mail = new Mail();

            mail.Parse(mailData);

            Database.InsertMail(mail);
            */

            /*for(int i = 0; i < args.Length; i++) {
                Console.WriteLine(i + ":" + args[i]);
            }

            try {
                POPServer hpop = new POPServer("0.0.0.0", int.Parse(args[1]), args[0]);

                hpop.Start();

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
           */

            /*
            try {
                SMTPServer server = new SMTPServer("0.0.0.0", int.Parse(args[1]), args[0]);

                server.Start();

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
            */

            string logfile = "/var/log/hellmaild/hellmail.log";

            string cert_path = "/etc/hellmaild/certs/cert.pfx";

            string smtp_address = "0.0.0.0";
            int smtp_port = 25;

            string pop_address = "0.0.0.0";
            int pop_port = 110;
            string password = "password";

            string[] flags = { "--config" };

            Dictionary<string, string> values = new Dictionary<string, string>();

            // Find all the available flags the user has set
            for (int i = 0; i < args.Length; i++) {

                // If the flag is a valid flag add it to the Dictionary, else terminate the program
                if (Array.IndexOf(flags, args[i]) >= 0) {
                    string memberName = args[i].Substring(2, 1).ToUpper() + args[i].Substring(3);
                    values.Add(memberName, args[i + 1]);
                    i++;
                }else {
                    Console.WriteLine("Unrecognized flag '" + args[i] + "'");
                    Environment.Exit(1);
                }

            }

            if(File.Exists(values["Config"])) {

                string file = File.ReadAllText(values["Config"]);

                logfile = Regex.Match(file, "^logfile = (.*)$", RegexOptions.Multiline).Groups[1].ToString();

                Logger.LOGFILE = logfile;

                cert_path = Regex.Match(file, "^cert_path = (.*)$", RegexOptions.Multiline).Groups[1].ToString();

                smtp_address = Regex.Match(file, "^smtp_address = (.*)$", RegexOptions.Multiline).Groups[1].ToString();

                if (!int.TryParse(Regex.Match(file, "^smtp_port = (.*)$", RegexOptions.Multiline).Groups[1].ToString(), out smtp_port)) {
                    Logger.Log("[INIT] Could not read 'smtp_port' in config");
                    Environment.Exit(1);
                }

                pop_address = Regex.Match(file, "^pop_address = (.*)$", RegexOptions.Multiline).Groups[1].ToString();

                if (!int.TryParse(Regex.Match(file, "^pop_port = (.*)$", RegexOptions.Multiline).Groups[1].ToString(), out pop_port)) {
                    Logger.Log("[INIT] Could not read 'pop_port' in config");
                    Environment.Exit(1);
                }

                password = Regex.Match(file, "^password = (.*)$", RegexOptions.Multiline).Groups[1].ToString();

            } else {
                Logger.LOGFILE = logfile;
                Logger.Log("[INIT] No config file was given, starting anyway using the default config");
            }

            Logger.Log("[INIT] Init successful - starting services..");

            try {

                POPServer hpop = new POPServer(pop_address, pop_port, cert_path);

                var popServerThread = new Thread(() => hpop.Start());
                popServerThread.Start();

            } catch (Exception ex) {
                Logger.Log("[HPOP][ERROR] hellmaild crashed with following exception: \n" + ex);
            }

            try {

                SMTPServer smtp = new SMTPServer(smtp_address, smtp_port, cert_path);

                var smtpServerThread = new Thread(() => smtp.Start());
                smtpServerThread.Start();

            } catch (Exception ex) {
                Logger.Log("[SMTP][ERROR] hellmaild crashed with following exception: \n" + ex);
            }

        }
    }
}
