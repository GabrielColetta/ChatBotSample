using Microsoft.Extensions.Hosting;
using SampleAI.Shared.Constants;

namespace SampleAI.Infrastructure.MongoDB.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddDatabaseClient(this IHostApplicationBuilder app)
    {
        app.AddMongoDBClient(GlobalVariables.InstanceName);

        return app;
    }
}
