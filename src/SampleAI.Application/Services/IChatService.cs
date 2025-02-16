


namespace SampleAI.Application.Services;

public interface IChatService
{
    IAsyncEnumerable<string> GenerateResponseAsync(string message, string conversationId, DateTime timestamp);
}
