namespace SampleAI.Shared.Interfaces;

public interface IChatHubClient
{
    Task ReceiveMessageAsync(string user, string content, string conversationId);
    Task<string> GetMessageAsync();
}
