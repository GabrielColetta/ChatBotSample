using Microsoft.Extensions.Logging;
using SampleAI.Shared.Interfaces;
using System.Net.Http.Json;

namespace SampleAI.Infrastructure.HTTP.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _client;
    private readonly ILogger<HttpService> _logger;

    public HttpService(HttpClient client, ILogger<HttpService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken)
        where TResponse : class
    {
        try
        {
            var response = await _client.GetAsync(endpoint, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
            }

            return default;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending a GET request to {Endpoint}", endpoint);
            return default;
        }
    }
}
