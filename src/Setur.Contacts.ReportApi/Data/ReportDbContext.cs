using Microsoft.EntityFrameworkCore;
using Setur.Contacts.Domain.Entities;

namespace Setur.Contacts.ReportApi.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options)
    {
    }

    public DbSet<Report> Reports { get; set; }
    public DbSet<ReportDetail> ReportDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RequestedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Parameters).IsRequired();
            entity.Property(e => e.Summary).IsRequired();
        });

        modelBuilder.Entity<ReportDetail>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Location).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PersonCount).IsRequired();
            entity.Property(e => e.PhoneCount).IsRequired();
            entity.Property(e => e.EmailCount).IsRequired();
            entity.HasOne(e => e.Report)
                .WithMany(e => e.ReportDetails)
                .HasForeignKey(e => e.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}