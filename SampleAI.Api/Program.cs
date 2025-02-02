using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SampleAI.Api.Hubs;
using SampleAI.IoC.Extensions;
using SampleAI.Shared.Constants;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var kernelBuilder = Kernel.CreateBuilder();
        kernelBuilder.AddOllamaChatCompletion(GetModel(), CreateUri());

        var kernel = kernelBuilder.Build();
        var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

        var builder = WebApplication.CreateBuilder(args);
        builder.AddDefaultApplications();
        builder.Services
            .AddDefaultServices()
            .AddLogging()
            .AddCors(options => options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()))
            .AddScoped(_ => chatCompletionService);

        builder.Services.AddSignalR();

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