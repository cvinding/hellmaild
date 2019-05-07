using HellMail.Domain;
using Microsoft.EntityFrameworkCore;

namespace HellMail.Data {

    public class HellMailContext : DbContext {

        public DbSet<Mail> Mails { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Mail_User> Mails_Users { get; set; }
        public DbSet<Hidden_Mails> Hidden_Mails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySql("Server=localhost;Database=hellmail;User=phpmyadmin;Password=admin;");
            base.OnConfiguring(optionsBuilder);
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Mail_User>()
                .Property(mu => mu.recipient_type)
                .HasDefaultValue(0);

            modelBuilder.Entity<Hidden_Mails>()
                .Property(hm => hm.hidden)
                .HasDefaultValue(0);
        }

    }
}
