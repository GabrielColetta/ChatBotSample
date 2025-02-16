using SampleAI.Shared.Models;

namespace SampleAI.Shared.Interfaces;

public interface IChatHubClient
{
    IAsyncEnumerable<ChatHistoryResponse> ReceiveMessageAsync(string message, string conversationId);
}
