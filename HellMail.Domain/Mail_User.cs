using System;
namespace HellMail.Domain {

    public class Mail_User {

        public int id { get; set; }
        public Mail mail_ { get; set; }
        public User from_user_ { get; set; }
        public User to_user_ { get; set; }
        public int recipient_type { get; set; }

    }
}
