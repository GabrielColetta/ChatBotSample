using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using SampleAI.Infrastructure.HTTP.Services;
using SampleAI.Shared.Configurations;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Infrastructure.HTTP.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCustomHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        var httpConfiguration = configuration.GetRequiredSection("http").Get<HttpConfiguration>();

        services.AddHttpClient<IHttpService, HttpService>(client =>
            {
                client.BaseAddress = new Uri(httpConfiguration!.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });
            //.SetHandlerLifetime(TimeSpan.FromMinutes(5))
            //.AddPolicyHandler(GetRetryPolicy());
        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }
}
