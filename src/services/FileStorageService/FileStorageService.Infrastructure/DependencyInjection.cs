using FileStorageService.Application.Handlers;
using FileStorageService.Application.Interfaces;
using FileStorageService.Domain.Repositories;
using FileStorageService.Infrastructure.Persistence;
using FileStorageService.Infrastructure.Repositories;
using FileStorageService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorageService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<FileDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("FileDb")));

        services.AddScoped<IFileMetadataRepository, FileMetadataRepository>();
        services.AddScoped<IUnitOfWork,            UnitOfWork>();

        // Storage Provider - Defaulting to Local
        services.AddSingleton<IStorageProvider, LocalFileStorageProvider>();

        return services;
    }
}
