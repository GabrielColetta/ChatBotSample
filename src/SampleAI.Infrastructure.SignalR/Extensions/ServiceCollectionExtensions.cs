using Microsoft.Extensions.DependencyInjection;
using SampleAI.Infrastructure.SignalR.Services;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Infrastructure.SignalR.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebSocket(this IServiceCollection services)
    {
        services.AddSignalR();
        services.AddWebSocketServices();
        return services;
    }
    
    public static IServiceCollection AddWebSocketServices(this IServiceCollection services)
    {
        services.AddSingleton<IWebSocketService, HubConnectionService>();
        return services;
    }
}
