using Microsoft.Extensions.Hosting;
using SampleAI.Infrastructure.MongoDB.Extensions;

namespace SampleAI.IoC.Extensions;

public static class HostApplicationBuilderExtensions
{
    public static IHostApplicationBuilder AddDefaultApplications(this IHostApplicationBuilder app)
    {
        app.AddDatabaseClient();

        return app;
    }
}
