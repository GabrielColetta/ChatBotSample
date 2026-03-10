using Microsoft.Extensions.Configuration;
using SampleAI.AppHost.Constants;
using SampleAI.Shared.Configurations;
using SampleAI.Shared.Constants;

namespace SampleAI.AppHost;

internal class Program
{
    private const string EnvironmentUserName = "MONGODB_INITDB_ROOT_USERNAME";
    private const string EnvironmentPassword = "MONGODB_INITDB_ROOT_PASSWORD";

    private static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var languageModelConfiguration = builder.Configuration
            .GetSection(ModelConfiguration.SectionName)
            .Get<ModelConfiguration>()!;

        var databaseConfiguration = builder.Configuration
            .GetSection(DatabaseConfiguration.SectionName)
            .Get<DatabaseConfiguration>()!;

        var ollama = builder.AddOllama(AppHostNames.Ollama, languageModelConfiguration.Port)
            .WithImage(GlobalVariables.OllamaImage)
            .WithGPUSupport()
            .WithDataVolume();

        var model = ollama.AddModel(GlobalVariables.Model, languageModelConfiguration.Model);
        var embeddingModel = ollama.AddModel(GlobalVariables.EmbeddingModel, languageModelConfiguration.EmbeddingModel);

        var password = builder.AddParameter(AppHostNames.DatabasePassword, secret: true);

        var mongoDb = builder.AddMongoDB(GlobalVariables.DatabaseName, port: Convert.ToInt32(databaseConfiguration.Port), password: password)
            .WithImage(GlobalVariables.DatabaseImage)
            .WithLifetime(ContainerLifetime.Persistent)
            .WithDataVolume()
            .WithContainerRuntimeArgs("--hostname", databaseConfiguration.Hostname)
            .WithEnvironment(EnvironmentUserName, AppHostNames.DatabaseUser)
            .WithEnvironment(EnvironmentPassword, password);

        var api = builder.AddProject<Projects.SampleAI_Api>(AppHostNames.Api)
            .WithReference(model)
            .WithReference(embeddingModel)
            .WithReference(mongoDb)
            .WaitFor(model)
            .WaitFor(embeddingModel)
            .WaitFor(mongoDb)
            .WithEnvironment(GlobalVariables.Model, languageModelConfiguration.Model)
            .WithEnvironment(GlobalVariables.EmbeddingModel, languageModelConfiguration.EmbeddingModel);

        builder.AddJavaScriptApp(AppHostNames.UI, "../SampleAI.UI", "start")
            .WithReference(api)
            .WaitFor(api)
            .WithExternalHttpEndpoints();

        builder.Build().Run();
    }
}