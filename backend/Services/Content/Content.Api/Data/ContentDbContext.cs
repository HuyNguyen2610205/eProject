using Content.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Content.Api.Data;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options)
    {
    }

    public DbSet<Ngo> Ngos => Set<Ngo>();
    public DbSet<Cause> Causes => Set<Cause>();
    public DbSet<NgoCause> NgoCauses => Set<NgoCause>();
    public DbSet<Programme> Programmes => Set<Programme>();
    public DbSet<ProgrammeRegistration> ProgrammeRegistrations => Set<ProgrammeRegistration>();
    public DbSet<AboutPage> AboutPages => Set<AboutPage>();
    public DbSet<Partner> Partners => Set<Partner>();
    public DbSet<Gallery> Galleries => Set<Gallery>();
    public DbSet<Query> Queries => Set<Query>();
    public DbSet<QueryReply> QueryReplies => Set<QueryReply>();
    public DbSet<EmailInvitation> EmailInvitations => Set<EmailInvitation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // NgoCause (Many-to-Many)
        modelBuilder.Entity<NgoCause>(entity =>
        {
            entity.ToTable("NgoCauses");
            entity.HasKey(nc => new { nc.NgoId, nc.CauseId });

            entity.HasOne(nc => nc.Ngo)
                .WithMany(n => n.NgoCauses)
                .HasForeignKey(nc => nc.NgoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(nc => nc.Cause)
                .WithMany(c => c.NgoCauses)
                .HasForeignKey(nc => nc.CauseId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Programme
        modelBuilder.Entity<Programme>(entity =>
        {
            entity.ToTable("Programmes");
            entity.HasKey(p => p.Id);

            entity.HasOne(p => p.Ngo)
                .WithMany(n => n.Programmes)
                .HasForeignKey(p => p.NgoId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.NgoId);
            entity.HasIndex(p => new { p.Status, p.StartDate });
        });

        // ProgrammeRegistration
        modelBuilder.Entity<ProgrammeRegistration>(entity =>
        {
            entity.ToTable("ProgrammeRegistrations");
            entity.HasKey(pr => pr.Id);

            entity.HasOne(pr => pr.Programme)
                .WithMany(p => p.Registrations)
                .HasForeignKey(pr => pr.ProgrammeId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(pr => new { pr.ProgrammeId, pr.UserId })
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL");

            entity.HasIndex(pr => new { pr.ProgrammeId, pr.Email });
        });

        // AboutPage
        modelBuilder.Entity<AboutPage>(entity =>
        {
            entity.ToTable("AboutPages");
            entity.HasKey(a => a.Id);
            entity.HasIndex(a => a.Slug).IsUnique();
        });

        // Partner
        modelBuilder.Entity<Partner>(entity =>
        {
            entity.ToTable("Partners");
            entity.HasKey(p => p.Id);
            entity.HasIndex(p => new { p.IsActive, p.DisplayOrder });
        });

        // Gallery
        modelBuilder.Entity<Gallery>(entity =>
        {
            entity.ToTable("Galleries");
            entity.HasKey(g => g.Id);

            entity.HasOne(g => g.Programme)
                .WithMany(p => p.Galleries)
                .HasForeignKey(g => g.ProgrammeId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasIndex(g => g.ProgrammeId);
        });

        // Query
        modelBuilder.Entity<Query>(entity =>
        {
            entity.ToTable("Queries");
            entity.HasKey(q => q.Id);
            entity.HasIndex(q => new { q.Status, q.CreatedAt });
            entity.HasIndex(q => q.UserId);
        });

        // QueryReply
        modelBuilder.Entity<QueryReply>(entity =>
        {
            entity.ToTable("QueryReplies");
            entity.HasKey(qr => qr.Id);

            entity.HasOne(qr => qr.Query)
                .WithMany(q => q.Replies)
                .HasForeignKey(qr => qr.QueryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(qr => qr.QueryId);
        });

        // EmailInvitation
        modelBuilder.Entity<EmailInvitation>(entity =>
        {
            entity.ToTable("EmailInvitations");
            entity.HasKey(ei => ei.Id);
            entity.HasIndex(ei => ei.SenderUserId);
        });
    }
}
