using System;
namespace HellMail.Domain {

    public class Hidden_Mails {

        public int id { get; set; }
        public Mail mail_ { get; set; }
        public User user_ { get; set; }
        public int hidden { get; set; }

    }
}
