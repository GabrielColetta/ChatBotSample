
namespace SampleAI.Shared.Interfaces;

public interface IWebSocketService : IAsyncDisposable
{
    IAsyncEnumerable<TResponse> ReceiveMessageAsync<TResponse>(string content, string conversationId, CancellationToken cancellationToken);
}
