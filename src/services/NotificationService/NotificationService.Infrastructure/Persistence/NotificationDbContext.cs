using NotificationService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace NotificationService.Infrastructure.Persistence;

public sealed class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<Notification>         Notifications => Set<Notification>();
    public DbSet<NotificationTemplate> Templates     => Set<NotificationTemplate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(builder =>
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Recipient).IsRequired().HasMaxLength(256);
            builder.Property(n => n.Subject).IsRequired().HasMaxLength(256);
            builder.Property(n => n.Metadata).HasColumnType("jsonb");
        });

        modelBuilder.Entity<NotificationTemplate>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.HasIndex(t => t.Name).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
