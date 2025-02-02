using Microsoft.Extensions.DependencyInjection;
using SampleAI.Infrastructure.MongoDB.Extensions;

namespace SampleAI.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultServices(this IServiceCollection services)
    {
        services.AddDatabaseServices();

        return services;
    }
}
