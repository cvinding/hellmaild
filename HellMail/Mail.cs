using System;
using System.Collections.Generic;

namespace HellMail {

    public class Mail {

        private readonly string[] required = { "to", "from", "subject" };
        private readonly string[] optional = { "cc", "bcc" };

        // Required mail tags
        public string To { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }

        // Optional mail tags
        public List<string> Cc = new List<string>();
        public List<string> Bcc = new List<string>();

        public string Message { get; set; }
     
        public void Parse(string SMTPDataMail) {
            if(SMTPDataMail.Length <= 0) {
                throw new Exception("Cannot parse empty mail");
            }

            try {
                Validate(SMTPDataMail);

            } catch (Exception ex) {
                Console.WriteLine(ex);
            }
        }

        private void Validate(string SMTPDataMail) {
            int index = -1;

            int bodyIndex = SMTPDataMail.IndexOf("\n\n", StringComparison.OrdinalIgnoreCase) + 2;

            string emailTags = SMTPDataMail.Substring(0, bodyIndex);

            // Check for required tags
            foreach (string tag in required) {
            
                if ((index = emailTags.IndexOf(tag + ":", StringComparison.OrdinalIgnoreCase)) == -1) {
                    throw new Exception("Required email tag is missing");
                }

                string data = SMTPDataMail.Substring(index + (tag + ":").Length + 1);

                int endIndex = data.IndexOf("\n", StringComparison.OrdinalIgnoreCase);

                SetPropValue(tag, data.Substring(0, endIndex));
                //Console.WriteLine(data.Substring(0, endIndex));
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
                    SetPropValue(tag, data.Substring(0, endIndex));
                } else {
                    SetFieldValue(tag, data.Substring(0, endIndex));
                }
            }

            Message = SMTPDataMail.Substring(bodyIndex, SMTPDataMail.Substring(bodyIndex).IndexOf("\n.\n", StringComparison.OrdinalIgnoreCase));
        }

        // SetPropValue() is used for setting property values
        private void SetPropValue(string name, string value) {
            string propName = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

            var prop = GetType().GetProperty(propName);

            if (prop.CanWrite) {
                prop.SetValue(this, value);
            }
        }

        // SetFieldValue() is used for adding entries into our List<string>
        private void SetFieldValue(string name, string value) {
            string propName = name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();

            var values = value.Replace(" ", "").Split(",");

            var prop = GetType().GetField(propName);

            List<string> list = (List<string>)prop.GetValue(this);

            for (int i = 0; i < values.Length; i++) {

                list.Add(values[i]);
            }

            prop.SetValue(this, list);
        }



    }
}
