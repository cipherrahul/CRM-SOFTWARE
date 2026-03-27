using NotificationService.Application.Interfaces;
using NotificationService.Domain.Repositories;
using NotificationService.Infrastructure.Persistence;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace NotificationService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("NotificationDb")));

        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<ITemplateRepository,     TemplateRepository>();
        services.AddScoped<IUnitOfWork,            UnitOfWork>();

        // Communication Providers
        services.AddScoped<IEmailService, MockEmailService>();
        services.AddScoped<ISmsService,   MockSmsService>();

        return services;
    }
}
