namespace SampleAI.Shared.Models;

public record ChatHistoryModel(string ChatRole, string Message, string ConversationId, DateTime Date)
{
    public const string DocumentName = "chatHistory";
}

