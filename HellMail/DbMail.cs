using System;
using System.Collections.Generic;

namespace HellMail {

    public class DbMail {

        // Required mail tags
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        // Optional mail tags
        public List<string> Cc = new List<string>();
        public List<string> Bcc = new List<string>();

        public string Message { get; set; }

        public DbMail(Domain.Mail mail, List<Domain.Mail_User> mail_user) {

            Subject = mail.subject;
            Message = mail.message;

            From = mail_user[0].from_user_.email;

            for (int i = 0; i < mail_user.Count; i++) {

                if(mail_user[i].recipient_type == 0) {

                    To = mail_user[i].to_user_.email;

                } else if(mail_user[i].recipient_type == 1) {

                    Cc.Add(mail_user[i].to_user_.email);

                } else if(mail_user[i].recipient_type == 2) {

                    Bcc.Add(mail_user[i].to_user_.email);
                }

            }

        }

        public string FormatMail() {
            string mail = String.Empty;

            mail += "To: " + To + "\n";
            mail += "From: " + From + "\n";
            mail += "Subject: " + Subject + "\n";

            if(Cc.Count > 0) {

                string cc = "Cc: ";

                foreach (string email in Cc) {
                    cc += email + ",";
                }

                mail += cc.TrimEnd(',') + "\n";
            }

            if (Bcc.Count > 0) {
                string bcc = "Bcc: ";

                foreach (string email in Bcc)
                {
                    bcc += email + ",";
                }

                mail += bcc.TrimEnd(',') + "\n";
            }

            mail += "\n\"" + Message.Replace("\"","\\\"", StringComparison.CurrentCulture) + "\"\n.\n\n";

            return mail;
        }

    }

}
