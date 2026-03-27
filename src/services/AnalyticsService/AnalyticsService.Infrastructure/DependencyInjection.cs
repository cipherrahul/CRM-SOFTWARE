using AnalyticsService.Application.Handlers;
using AnalyticsService.Domain.Repositories;
using AnalyticsService.Infrastructure.Persistence;
using AnalyticsService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AnalyticsService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AnalyticsDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AnalyticsDb")));

        services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
        services.AddScoped<IMetricRepository,    MetricRepository>();
        services.AddScoped<IUnitOfWork,          UnitOfWork>();

        return services;
    }
}
