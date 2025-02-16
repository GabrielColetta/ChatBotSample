namespace SampleAI.Shared.Models;

public record ChatHistoryResponse(string ChatRole, string Content, string? ConversationId, DateTime Date);
