using System.Threading.Channels;
using SampleAI.Api.Hubs;
using SampleAI.IoC.Extensions;
using SampleAI.Shared.Constants;
using Microsoft.Extensions.AI;
using OllamaSharp;
using SampleAI.Api.Workers;
using SampleAI.Shared.Models;

namespace SampleAI.Api;

public class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddDefaultApplications();
        builder.Services
            .AddApiServices(builder.Configuration)
            .AddLogging()
            .AddCors(options => options
                .AddDefaultPolicy(b => b
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()));

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services
            .AddHealthChecks()
            .AddMongoDb()
            .AddUrlGroup(CreateUri(), "model");

        builder.Services.AddChatClient(x => new ChatClientBuilder(new OllamaApiClient(CreateUri(), GetModel()))
            .UseOpenTelemetry()
            .Build(x));
        
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

        await app.RunAsync();
    }

    private static string GetModel()
    {
        return Environment.GetEnvironmentVariable(GlobalVariables.Model)!;
    }

    private static Uri CreateUri()
    {
        var port = Environment.GetEnvironmentVariable(GlobalVariables.Port);
        return new Uri($"http://localhost:{port}");
    }
}