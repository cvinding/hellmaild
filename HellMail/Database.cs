using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HellMail.Data;
using HellMail.Domain;

namespace HellMail {

    public static class Database {
    
        public static bool InsertMail(Mail mail) {

            HellMailContext context = new HellMailContext();

            var users = context.Users
                .Where(u => u.email == mail.To || u.email == mail.From || mail.Cc.Contains(u.email) || mail.Cc.Contains(u.email))
                .Select(u => new User {
                    id = u.id,
                    email = u.email
                })
                .ToDictionary(u => u.email, u => u.id);
                

            // Create Mail
            Domain.Mail newMail = new Domain.Mail { subject = mail.Subject, message = mail.Message };

            context.Add(newMail);
            context.SaveChanges();

            Console.WriteLine(newMail.id);

            for (int i = 0; i < users.Count; i++) {
                Mail_User mail_user = new Mail_User { mail_ = newMail, from_user_ = new User { id = users[mail.From] },   };
            }

            // 
            //Mail_User mail_user = new Mail_User { from_user_ = user1, to_user_ = user2, mail_ = newMail, recipient_type = 0 };


            return true;
        }

    }
}
