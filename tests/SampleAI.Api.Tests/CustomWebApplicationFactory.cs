using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using NSubstitute;
using SampleAI.IoC.Extensions;
using Testcontainers.MongoDb;

namespace SampleAI.Api.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
    where TProgram : class
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder().Build();

    public CustomWebApplicationFactory()
    {
        _mongoDbContainer.StartAsync().GetAwaiter().GetResult();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services
                .AddDefaultServices()
                .Replace(new ServiceDescriptor
                (
                    serviceType: typeof(IMongoClient),
                    factory: _ => new MongoClient(_mongoDbContainer.GetConnectionString()),
                    lifetime: ServiceLifetime.Singleton
                ))
                .Replace(new ServiceDescriptor
                (
                    serviceType: typeof(IChatClient),
                    factory: _ => Substitute.For<IChatClient>(),
                    lifetime: ServiceLifetime.Singleton
                ));
        });

        builder.UseEnvironment("Development");
    }

    public async override ValueTask DisposeAsync()
    {
        await _mongoDbContainer.StopAsync();
        await _mongoDbContainer.DisposeAsync();
        await base.DisposeAsync();
    }
}
