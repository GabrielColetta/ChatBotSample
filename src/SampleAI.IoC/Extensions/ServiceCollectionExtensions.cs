using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleAI.Application.Extensions;
using SampleAI.Infrastructure.HTTP.Extensions;
using SampleAI.Infrastructure.MongoDB.Extensions;
using SampleAI.Infrastructure.SignalR.Extensions;

namespace SampleAI.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabaseServices(configuration)
            .AddApplicationServices()
            .AddWebSocket();

        return services;
    }

    public static IServiceCollection AddDefaultAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWebSocketServices()
            .AddCustomHttpClients(configuration);

        return services;
    }
}
