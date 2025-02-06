namespace SampleAI.Shared.Interfaces;

public interface IChatHubClient
{
    Task ReceiveMessageAsync(string user, string message, string conversationId);
    Task<string> GetMessageAsync();
}
