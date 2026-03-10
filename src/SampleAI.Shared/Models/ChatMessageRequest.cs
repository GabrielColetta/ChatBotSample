namespace SampleAI.Shared.Models;

public record ChatMessageRequest(string ConnectionId, Guid? ChatId, string UserPrompt);