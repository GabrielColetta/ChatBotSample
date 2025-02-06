using Microsoft.Extensions.DependencyInjection;
using SampleAI.Infrastructure.MongoDB.Context;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Infrastructure.MongoDB.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseContext, MongoContext>();

        return services;
    }
}
