using AIService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AIService.Infrastructure.Persistence;

public sealed class AiDbContext : DbContext
{
    public AiDbContext(DbContextOptions<AiDbContext> options) : base(options) { }

    public DbSet<AiModel> Models => Set<AiModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiModel>(builder =>
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
            builder.Property(m => m.Version).IsRequired().HasMaxLength(20);
            builder.HasIndex(m => new { m.Name, m.Version }).IsUnique();
            
            // Store Metadata as JSON-b in PostgreSQL
            builder.Property(m => m.Metadata).HasColumnType("jsonb");
        });

        base.OnModelCreating(modelBuilder);
    }
}
