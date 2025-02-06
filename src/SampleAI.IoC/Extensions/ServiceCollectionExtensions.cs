using Microsoft.Extensions.DependencyInjection;
using SampleAI.Application.Extensions;
using SampleAI.Infrastructure.MongoDB.Extensions;

namespace SampleAI.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddDatabaseServices().AddChatService();

        return services;
    }
}
