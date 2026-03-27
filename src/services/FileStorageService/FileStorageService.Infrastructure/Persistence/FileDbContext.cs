using FileStorageService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileStorageService.Infrastructure.Persistence;

public sealed class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options) { }

    public DbSet<FileMetadata> FileMetadata => Set<FileMetadata>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileMetadata>(builder =>
        {
            builder.HasKey(m => m.Id);
            builder.Property(m => m.FileName).IsRequired().HasMaxLength(256);
            builder.Property(m => m.FileUrl).IsRequired();
            builder.HasIndex(m => m.OwnerId);
            builder.HasIndex(m => m.RelatedEntityId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
