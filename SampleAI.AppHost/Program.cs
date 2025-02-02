using Microsoft.Extensions.Configuration;
using SampleAI.AppHost.Configurations;
using SampleAI.AppHost.Constants;
using SampleAI.Shared.Constants;

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

        var password = builder.AddParameter(AppHostNames.Password, secret: true);
        var mongoDb = builder.AddMongoDB(GlobalVariables.Database, password: password)
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

        builder.AddNpmApp(AppHostNames.UI, "../SampleAI.UI")
            .WithReference(api)
            .WaitFor(api)
            .WithExternalHttpEndpoints()
            .WithExternalHttpEndpoints()
            .PublishAsDockerFile();

        builder.Build().Run();
    }
}