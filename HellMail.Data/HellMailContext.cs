using HellMail.Domain;
using Microsoft.EntityFrameworkCore;

namespace HellMail.Data
{
    public class HellMailContext : DbContext
    {
        public DbSet<Mail> Mails { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseMySql("Server=localhost;Database=HellMail;User=phpmyadmin;Password=admin;");
            base.OnConfiguring(optionsBuilder);
        }

    }
}
