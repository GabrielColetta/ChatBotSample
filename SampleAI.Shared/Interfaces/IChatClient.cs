namespace SampleAI.Shared.Interfaces;

public interface IChatClient
{
    Task ReceiveMessageAsync(string user, string message, string conversationId);
    Task<string> GetMessageAsync();
}
