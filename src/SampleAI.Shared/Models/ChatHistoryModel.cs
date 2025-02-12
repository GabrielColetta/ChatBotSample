namespace SampleAI.Shared.Models;

public record ChatHistoryModel(string ChatRole, string Content, string ConversationId, DateTime Date)
{
    public const string DocumentName = "chatHistory";
}

