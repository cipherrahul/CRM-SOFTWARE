using CRMService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRMService.Infrastructure.Persistence;

public sealed class CrmDbContext : DbContext
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

    public DbSet<Lead>    Leads    => Set<Lead>();
    public DbSet<Deal>    Deals    => Set<Deal>();
    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CrmDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
