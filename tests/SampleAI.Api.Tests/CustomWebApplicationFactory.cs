using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NSubstitute;
using SampleAI.Shared.Configurations;
using SampleAI.Shared.Constants;
using Testcontainers.MongoDb;
using Testcontainers.Ollama;

namespace SampleAI.Api.Tests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>, IAsyncLifetime
    where TProgram : class
{
    private static readonly string _localModelsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OllamaTestContainer",
        "models");

    private static readonly ModelConfiguration _languageModelConfiguration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.Tests.json", optional: false, reloadOnChange: true)
        .Build()
        .GetSection(ModelConfiguration.SectionName)
        .Get<ModelConfiguration>()!;

    private static readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder(GlobalVariables.DatabaseImage)
        .WithEnvironment("MONGODB_INITDB_ROOT_USERNAME", "mongo")
        .WithEnvironment("MONGODB_INITDB_ROOT_PASSWORD", "mongo")
        .Build();

    private static readonly OllamaContainer _ollamaContainer = new OllamaBuilder("ollama/ollama:latest")
        .WithBindMount(_localModelsPath, "/root/.ollama")
        .Build();

    private static bool _initialized = false;
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Environment.SetEnvironmentVariable($"ConnectionStrings:{GlobalVariables.DatabaseName}", _mongoDbContainer.GetConnectionString());
        Environment.SetEnvironmentVariable($"ConnectionStrings:{GlobalVariables.EmbeddingModel}", GetConnectionString());
        Environment.SetEnvironmentVariable(GlobalVariables.EmbeddingModel, _languageModelConfiguration.EmbeddingModel);
        Environment.SetEnvironmentVariable(GlobalVariables.Model, _languageModelConfiguration.Model);

        builder.ConfigureTestServices(services =>
        {
            services
                .Replace(new ServiceDescriptor
                (
                    serviceType: typeof(IChatClient),
                    factory: _ => Substitute.For<IChatClient>(),
                    lifetime: ServiceLifetime.Singleton
                ));
        });

        builder.UseEnvironment("Development");
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            if (_initialized)
            {
                return;
            }

            if (!Directory.Exists(_localModelsPath))
            {
                Directory.CreateDirectory(_localModelsPath);
            }

            await _mongoDbContainer.StartAsync();
            await _ollamaContainer.StartAsync();

            await _ollamaContainer.ExecAsync(["ollama", "pull", _languageModelConfiguration.EmbeddingModel]);


            _initialized = true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static string GetConnectionString()
    {
        var host = _ollamaContainer.Hostname;
        var port = _ollamaContainer.GetMappedPublicPort(11434);
        return $"Endpoint=http://{host}:{port};Model={_languageModelConfiguration.EmbeddingModel}";
    }
}
