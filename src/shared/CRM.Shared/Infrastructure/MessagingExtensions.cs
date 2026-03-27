using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CRM.Shared.Infrastructure;

public static class MassTransitExtensions
{
    public static IServiceCollection AddCloudMessaging(
        this IServiceCollection services, 
        IConfiguration configuration,
        Action<IBusRegistrationConfigurator>? configureConsumers = null)
    {
        services.AddMassTransit(x =>
        {
            configureConsumers?.Invoke(x);

            x.UsingRabbitMq((context, cfg) =>
            {
                var host = configuration["RabbitMQ:Host"] ?? "localhost";
                var user = configuration["RabbitMQ:User"] ?? "guest";
                var pass = configuration["RabbitMQ:Pass"] ?? "guest";

                cfg.Host(host, "/", h =>
                {
                    h.Username(user);
                    h.Password(pass);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
