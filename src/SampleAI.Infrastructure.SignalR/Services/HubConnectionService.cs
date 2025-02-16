using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using SampleAI.Shared.Configurations;
using SampleAI.Shared.Interfaces;
using System.Runtime.CompilerServices;

namespace SampleAI.Infrastructure.SignalR.Services;

public sealed class HubConnectionService : IWebSocketService
{
    private readonly HubConnection _connection;

    public HubConnectionService(IOptions<SocketConfiguration> options)
    {
        var hubConnection = options.Value;
        _connection = new HubConnectionBuilder()
            .WithUrl(hubConnection.BaseUrl + hubConnection.Endpoint)
            .WithAutomaticReconnect()
            .Build();

        _connection.StartAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async IAsyncEnumerable<TResponse> ReceiveMessageAsync<TResponse>(
        string content,
        string conversationId,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await foreach (var response in _connection.StreamAsync<TResponse>("SendMessageAsync", content, conversationId, cancellationToken))
        {
            yield return response;
        }
    }
}
