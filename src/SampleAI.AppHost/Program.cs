using Microsoft.Extensions.Configuration;
using SampleAI.AppHost.Configurations;
using SampleAI.AppHost.Constants;
using SampleAI.Shared.Constants;

namespace SampleAI.AppHost;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var languageModelConfiguration = builder.Configuration
            .GetSection(LanguageModelConfiguration.SectionName)
            .Get<LanguageModelConfiguration>()!;

        var ollama = builder.AddOllama(AppHostNames.Ollama, languageModelConfiguration.Port)
            .WithGPUSupport()
            .WithDataVolume()
            .AddModel(languageModelConfiguration.Model);

        var mongoDb = builder.AddMongoDB(GlobalVariables.Database)
            .WithImage(GlobalVariables.DatabaseImage)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume()
            .AddDatabase(GlobalVariables.InstanceName);

        var api = builder.AddProject<Projects.SampleAI_Api>(AppHostNames.Api)
            .WithReference(ollama)
            .WithReference(mongoDb)
            .WaitFor(ollama)
            .WaitFor(mongoDb)
            .WithEnvironment(GlobalVariables.Port, languageModelConfiguration.Port.ToString())
            .WithEnvironment(GlobalVariables.Model, languageModelConfiguration.Model);

        builder.AddJavaScriptApp(AppHostNames.UI, "../SampleAI.UI", "start")
            .WithReference(api)
            .WaitFor(api)
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}