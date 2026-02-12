namespace SampleAI.Shared.Models;

public record ChatMessageRequest(string ConnectionId, string ConversationId, string UserPrompt);