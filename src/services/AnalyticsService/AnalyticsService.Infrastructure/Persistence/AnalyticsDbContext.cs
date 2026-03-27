using AnalyticsService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnalyticsService.Infrastructure.Persistence;

public sealed class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options) { }

    public DbSet<AnalyticsEvent> Events   => Set<AnalyticsEvent>();
    public DbSet<DailyMetric>    Metrics  => Set<DailyMetric>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AnalyticsEvent>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Payload).HasColumnType("jsonb");
            builder.HasIndex(e => e.Type);
            builder.HasIndex(e => e.OccurredAt);
        });

        modelBuilder.Entity<DailyMetric>(builder =>
        {
            builder.HasKey(m => m.Id);
            builder.HasIndex(m => new { m.Type, m.Date, m.Dimensions }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
