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
            .AddCors(options => options
                .AddDefaultPolicy(builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod()));

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services
            .AddHealthChecks()
            .AddMongoDb(GetMongoDBConnectionString(builder.Configuration))
            .AddUrlGroup(CreateUri(), "model");

        builder.Services.AddSignalR();
        builder.Services.AddChatClient(x => new OllamaChatClient(CreateUri(), GetModel()).AsBuilder()
            .UseOpenTelemetry()
            .Build(x));

        var app = builder.Build();

        app.UseCors()
            .UseRouting()
            .UseEndpoints(app => app.MapControllers())
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

    private static string GetMongoDBConnectionString(IConfiguration configuration)
    {
        return configuration.GetConnectionString(GlobalVariables.InstanceName)!;
    }

    private static Uri CreateUri()
    {
        var port = Environment.GetEnvironmentVariable(GlobalVariables.Port);
        return new Uri($"http://localhost:{port}");
    }
}