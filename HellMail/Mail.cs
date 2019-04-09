using System;
using System.Collections.Generic;

namespace HellMail {

    public class Mail {

        // Required mail tags
        private string to;
        private string from;
        private string subject;

        // Optional mail tags
        //private string date;
        private string message;
        //private List<string> cc;
        //private List<string> bcc;

        public Mail Parse(string SMTPDataMail) {
            if(SMTPDataMail.Length <= 0) {
                throw new Exception("Cannot parse empty mail");
            }

            //string[] required = { "to", "from", "subject" };
            //string[] optional = { "date", "cc", "bcc" };

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

            Console.WriteLine(SMTPDataMail);

            Console.WriteLine("Validate " + SMTPDataMail.IndexOf("To:", StringComparison.OrdinalIgnoreCase));


            if ((index = SMTPDataMail.IndexOf("To:", StringComparison.OrdinalIgnoreCase)) != -1) {
                Console.WriteLine("'To:' is at index " + index);
            }

        }



    }
}
