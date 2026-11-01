using Donation.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Donation.Api.Data;

public class DonationDbContext : DbContext
{
    public DonationDbContext(DbContextOptions<DonationDbContext> options) : base(options)
    {
    }

    public DbSet<Cause> Causes => Set<Cause>();
    public DbSet<Entities.Donation> Donations => Set<Entities.Donation>();
    public DbSet<DonationStatistic> DonationStatistics => Set<DonationStatistic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Cause
        modelBuilder.Entity<Cause>(entity =>
        {
            entity.ToTable("Causes");
            entity.HasKey(c => c.Id);
            entity.HasIndex(c => c.Slug).IsUnique();
            entity.HasIndex(c => new { c.IsActive, c.DisplayOrder });

            entity.Property(c => c.Name).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Slug).IsRequired().HasMaxLength(150);
            entity.Property(c => c.Description).HasMaxLength(1000);
            entity.Property(c => c.IconUrl).HasMaxLength(500);
            entity.Property(c => c.ImageUrl).HasMaxLength(500);
        });

        // Configure Donation
        modelBuilder.Entity<Entities.Donation>(entity =>
        {
            entity.ToTable("Donations");
            entity.HasKey(d => d.Id);
            entity.HasIndex(d => d.TransactionCode).IsUnique();
            entity.HasIndex(d => new { d.UserId, d.CreatedAt });
            entity.HasIndex(d => new { d.CauseId, d.Status });
            entity.HasIndex(d => new { d.Status, d.CreatedAt });

            entity.Property(d => d.TransactionCode).IsRequired().HasMaxLength(50);
            entity.Property(d => d.DonorName).IsRequired().HasMaxLength(150);
            entity.Property(d => d.DonorEmail).IsRequired().HasMaxLength(256);
            entity.Property(d => d.DonorPhone).HasMaxLength(50);
            entity.Property(d => d.Amount).HasColumnType("decimal(18,2)");
            entity.Property(d => d.PaymentMethod).IsRequired().HasMaxLength(50);
            entity.Property(d => d.CardLastFour).HasMaxLength(10);
            entity.Property(d => d.Status).IsRequired().HasMaxLength(50);
            entity.Property(d => d.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Cause)
                .WithMany(c => c.Donations)
                .HasForeignKey(d => d.CauseId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure View mapping
        modelBuilder.Entity<DonationStatistic>(entity =>
        {
            entity.ToView("vw_DonationStatistics");
        });
    }
}
