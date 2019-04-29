using System;
using System.Collections.Generic;

namespace HellMail {

    public class Mail {

        private readonly string[] required = { "to", "from", "subject" };
        private readonly string[] optional = { "date", "cc", "bcc" };

        // Required mail tags
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        // Optional mail tags
        public string Date { get; set; }
        public List<string> Cc;
        public List<string> Bcc;

        public string Message { get; set; }
     
        public Mail Parse(string SMTPDataMail) {
            if(SMTPDataMail.Length <= 0) {
                throw new Exception("Cannot parse empty mail");
            }

            /*
            To: christian@hellmail.dk
            From: kent@hellmail.dk
            Subject: Test
            Cc: tubbe@hellmail.dk, lmao@hellmail.dk
            Bcc: tuppe@hellmail.dk, uffeholm@hellmail.dk

            Hej med jer alle sammen
            .           

            */

            /*
            Find ud hvornår email tags stopper, og hvornår message starter 
            Find mail tags
            Parse mail tags
            Returner Mail klassen
            */

            try {

                Validate(SMTPDataMail);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }

            return this;
        }

        private void Validate(string SMTPDataMail) {
            int index = -1;

            int bodyIndex = SMTPDataMail.IndexOf("\n\n", StringComparison.OrdinalIgnoreCase);

            string emailTags = SMTPDataMail.Substring(0, bodyIndex);

            // Check for required tags
            foreach (string tag in required) {
            
                if ((index = emailTags.IndexOf(tag + ":", StringComparison.OrdinalIgnoreCase)) == -1) {
                    throw new Exception("Required email tag is missing");
                }

                string data = SMTPDataMail.Substring(index + (tag + ":").Length + 1);

                int endIndex = data.IndexOf("\n", StringComparison.OrdinalIgnoreCase);

                SetValidatedTag(tag, data.Substring(0, endIndex));
                Console.WriteLine(data.Substring(0, endIndex));
            }

            index = -1;

            // Check for optional tags
            foreach(string tag in optional) {

                if ((index = emailTags.IndexOf(tag + ":", StringComparison.OrdinalIgnoreCase)) == -1) {
                    continue;
                }

                string data = SMTPDataMail.Substring(index + (tag + ":").Length + 1);

                int endIndex = data.IndexOf("\n", StringComparison.OrdinalIgnoreCase);

                string propName = tag.Substring(0, 1).ToUpper() + tag.Substring(1).ToLower();

                if (GetType().GetProperty(propName) is string) {
                    SetValidatedTag(tag, data.Substring(0, endIndex));
                } else {
                    AddToValidatedTag(tag, data.Substring(0, endIndex));
                }

                Console.WriteLine(data.Substring(0, endIndex));

            }

            //


            /*if ((index = SMTPDataMail.IndexOf("To:", StringComparison.OrdinalIgnoreCase)) != -1) {
                Console.WriteLine("'To:' is at index " + index);
            }*/

        }

        private void SetValidatedTag(string name, string value) {
            string propName = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

            var prop = GetType().GetProperty(propName);

            if (prop.CanWrite) {
                prop.SetValue(this, value);
            }
        }

        private void AddToValidatedTag(string name, string value) {
            string propName = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

            var prop = GetType().GetProperty(propName);

            if (prop.CanWrite) {
                List<string> list = (List<string>) prop.GetValue(this, null);
                list.Add(value);
            }
        }



    }
}
