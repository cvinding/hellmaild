using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using HellMail.Data;
using HellMail.Domain;

namespace HellMail {

    public static class Database {

        // InsertMail() is used for inserting the correct number of rows in the appropiate tables
        public static bool InsertMail(Mail mail) {

            // Init db context
            HellMailContext context = new HellMailContext();

            // Select all users that needs to recieve this mail
            var users = context.Users
                .Where(u => u.email == mail.To || u.email == mail.From || mail.Cc.Contains(u.email) || mail.Bcc.Contains(u.email))
                .Select(u => new User {
                    id = u.id,
                    email = u.email
                })
                .ToDictionary(u => u.email, u => u.id);
                
            // Create Mail
            Domain.Mail newMail = new Domain.Mail { subject = mail.Subject, message = mail.Message };

            // Add mail to context and insert mail
            context.Add(newMail);
            context.SaveChanges();

            int recipient_type = -1;

            List<Mail_User> mail_users = new List<Mail_User>();
            List<Hidden_Mails> hidden_mails = new List<Hidden_Mails>();

            // Loop through every mail reciever 
            foreach(KeyValuePair<string, int> user in users) {
                
                // Create the hidden_mails entry and add it to our List
                Hidden_Mails hidden_mail = new Hidden_Mails {
                    mail_ = newMail,
                    user_ = context.Users.Single(u => u.id == user.Value)
                };
                
                hidden_mails.Add(hidden_mail);

                // Skip from user
                if(user.Key == mail.From && user.Key != mail.To && !mail.Cc.Contains(user.Key) && !mail.Bcc.Contains(user.Key)) {
                    continue;
                }

                // Get recipient_type
                if(user.Key == mail.To) {
                    recipient_type = 0;
                } else if (mail.Cc.Contains(user.Key)) {
                    recipient_type = 1;
                } else if (mail.Bcc.Contains(user.Key)) {
                    recipient_type = 2;
                }

                // Create the mails_users entry and add it to our List
                Mail_User mail_user = new Mail_User { 
                    mail_ = newMail, 
                    from_user_ = context.Users.Single(u => u.id == users[mail.From]), 
                    to_user_ = context.Users.Single(u => u.id == user.Value), 
                    recipient_type = recipient_type   
                };
                
                mail_users.Add(mail_user);

            }
            
            // Add range and insert all entries
            context.AddRange(mail_users);
            context.AddRange(hidden_mails);

            context.SaveChanges();

            return true;
        }

        public static List<int> GetMailIDList(string email, string type, int startIndex, int endIndex) {
            
            // Init db context
            HellMailContext context = new HellMailContext();

            List<int> mails = new List<int>();

            if (type == "SENT") {

                mails = context.Mails_Users
                .Where(mu => mu.from_user_ == context.Users.Where(u => u.email == email).Single() && context.Hidden_Mails.Where(h => h.mail_ == mu.mail_ && h.user_ == mu.from_user_).Single().hidden == 0)
                .Select(mu => mu.mail_.id)
                .Skip(startIndex - 1)
                .Take((endIndex - startIndex) + 1)
                .ToList();

            } else if(type == "RECIEVED") {

                mails = context.Mails_Users
                .Where(mu => mu.to_user_ == context.Users.Where(u => u.email == email).Single() && context.Hidden_Mails.Where(h => h.mail_ == mu.mail_ && h.user_ == mu.to_user_).Single().hidden == 0)
                .Select(mu => mu.mail_.id)
                .Skip(startIndex - 1)
                .Take((endIndex - startIndex) + 1)
                .ToList();

            }

            return mails;
        }

        public static List<int> GetMailIDList(string currentUserEmail, string type) {

            // Init db context
            HellMailContext context = new HellMailContext();

            List<int> mails = new List<int>();

            if (type == "SENT") {

                mails = context.Mails_Users
                .Where(mu => mu.from_user_ == context.Users.Where(u => u.email == currentUserEmail).Single() && context.Hidden_Mails.Where(h => h.mail_ == mu.mail_ && h.user_ == mu.from_user_).Single().hidden == 0)
                .Select(mu => mu.mail_.id)
                .ToList();

            } else if (type == "RECIEVED") {

                mails = context.Mails_Users
                .Where(mu => mu.to_user_ == context.Users.Where(u => u.email == currentUserEmail).Single() && context.Hidden_Mails.Where(h => h.mail_ == mu.mail_ && h.user_ == mu.to_user_).Single().hidden == 0)
                .Select(mu => mu.mail_.id)
                .ToList();
            }

            return mails;
        }

        public static List<DbMail> GetMails(string currentUserEmail, List<string> mailIds) {

            List<DbMail> dbMails = new List<DbMail>();

            // Init db context
            HellMailContext context = new HellMailContext();

            var mails = context.Mails
                .Where(m => mailIds.Contains(m.id.ToString()))
                .ToList();
                
            for (int i = 0; i < mails.Count; i++) {

                var mail_users = context.Mails_Users
                    .Where(mu => mu.mail_ == mails[i] && (mu.from_user_.email == currentUserEmail || mu.to_user_.email == currentUserEmail) && context.Hidden_Mails.Where(h => h.mail_ == mu.mail_ && h.user_ == mu.to_user_).Single().hidden == 0)
                    .Select(mu => new Mail_User {
                        id = mu.id,
                        mail_ = mu.mail_,
                        from_user_ = mu.from_user_,
                        to_user_ = mu.to_user_,
                        recipient_type = mu.recipient_type
                    })
                    .ToList();
                    
                if(mail_users.Count > 0) {
                    dbMails.Add(new DbMail(mails[i], mail_users));
                }

            }

            return dbMails;
        }

    }
}
