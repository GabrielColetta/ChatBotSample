using Microsoft.Extensions.AI;
using SampleAI.Api.Hubs;
using SampleAI.Api.Workers;
using SampleAI.Infrastructure.MongoDB.Services;
using SampleAI.IoC.Extensions;
using SampleAI.Shared.Constants;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;
using System.Threading.Channels;

namespace SampleAI.Api;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddApiServices(builder.Configuration)
            .AddLogging()
            .AddCors(options => options
                .AddDefaultPolicy(b => b
                    .SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)
                    .AllowAnyHeader()
                    .AllowAnyMethod()));

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services
            .AddHealthChecks()
            .AddMongoDb()
            .AddUrlGroup(CreateUri(), "model");

        builder.AddOllamaApiClient(GlobalVariables.Model, x => x.SelectedModel = GetModel())
            .AddChatClient()
            .UseOpenTelemetry()
            .UseLogging();

        builder.AddOllamaApiClient(GlobalVariables.EmbeddingModel, x => x.SelectedModel = GetEmbeddingModel())
            .AddEmbeddingGenerator()
            .UseOpenTelemetry()
            .UseLogging();

        builder.Services.AddSingleton(Channel.CreateBounded<ChatMessageRequest>(new BoundedChannelOptions(100)
        {
            FullMode = BoundedChannelFullMode.Wait
        }));
        builder.Services.AddSingleton(sp => sp.GetRequiredService<Channel<ChatMessageRequest>>().Reader);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<Channel<ChatMessageRequest>>().Writer);

        builder.Services.AddHostedService<ChatProcessorWorker>();

        var app = builder.Build();

        app.UseCors()
            .UseRouting()
            .UseEndpoints(a => a.MapControllers())
            .UseHealthChecks("/health");

        app.MapHub<ChatHub>("/chat");


        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        await WarmupDatabase(app);

        await app.RunAsync();
    }

    private static async Task WarmupDatabase(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        await initializer.InitializeAsync();
    }

    private static string GetModel()
    {
        return Environment.GetEnvironmentVariable(GlobalVariables.Model)!;
    }

    private static string GetEmbeddingModel()
    {
        return Environment.GetEnvironmentVariable(GlobalVariables.EmbeddingModel)!;
    }

    private static Uri CreateUri()
    {
        return new Uri($"http://localhost:11434");
    }
}