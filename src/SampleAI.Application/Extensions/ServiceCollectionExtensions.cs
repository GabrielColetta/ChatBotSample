using Microsoft.Extensions.DependencyInjection;
using SampleAI.Application.Services;
using SampleAI.Application.Services.Impl;

namespace SampleAI.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChatService(this IServiceCollection services)
    {
        services.AddScoped<IChatService, ChatService>();
        return services;
    }
}
