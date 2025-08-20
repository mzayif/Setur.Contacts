using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ContactApi.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<CommunicationInfo> CommunicationInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.CreateUser).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Company).HasMaxLength(100);
            });

            modelBuilder.Entity<CommunicationInfo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreateDate).IsRequired();
                entity.Property(e => e.CreateUser).IsRequired();
                entity.Property(e => e.Value).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Contact)
                      .WithMany(e => e.CommunicationInfos)
                      .HasForeignKey(e => e.ContactId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
