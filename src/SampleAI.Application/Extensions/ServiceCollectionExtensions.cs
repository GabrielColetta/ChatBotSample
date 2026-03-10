using Microsoft.Extensions.DependencyInjection;
using SampleAI.Application.Services;
using SampleAI.Application.Services.Impl;

namespace SampleAI.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();

        return services;
    }
}
