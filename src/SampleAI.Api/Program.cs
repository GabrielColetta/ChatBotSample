using SampleAI.Api.Hubs;
using SampleAI.IoC.Extensions;
using SampleAI.Shared.Constants;
using Microsoft.Extensions.AI;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddDefaultApplications();
        builder.Services
            .AddDefaultServices()
            .AddLogging()
            .AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        builder.Services.AddSignalR();
        builder.Services.AddChatClient(x => new OllamaChatClient(CreateUri(), GetModel()).AsBuilder()
            .UseOpenTelemetry()
            .Build(x));

        var app = builder.Build();

        app.UseCors();
        app.MapHub<ChatHub>("/chat");

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