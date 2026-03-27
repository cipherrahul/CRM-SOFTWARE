using AIService.Application.Interfaces;
using AIService.Domain.Repositories;
using AIService.Infrastructure.Engines;
using AIService.Infrastructure.Persistence;
using AIService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AIService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AiDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("AiDb")));

        services.AddScoped<IAiModelRepository, AiModelRepository>();
        services.AddScoped<IUnitOfWork,        UnitOfWork>();

        // AI Engines
        services.AddScoped<ILeadScoringEngine,      LeadScoringEngine>();
        services.AddScoped<ISalesForecastingEngine, SalesForecastingEngine>();
        services.AddScoped<ISegmentationEngine,     SegmentationEngine>();
        services.AddScoped<IChatbotEngine,          ChatbotEngine>();
        services.AddScoped<IEmailGeneratorEngine,   EmailGeneratorEngine>();
        services.AddScoped<IRecommendationEngine,   RecommendationEngine>();

        // Settings
        services.Configure<ChatbotSettings>(configuration.GetSection(ChatbotSettings.Section));

        return services;
    }
}
