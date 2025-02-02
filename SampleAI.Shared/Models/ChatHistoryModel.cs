namespace SampleAI.Shared.Models;

public record ChatHistoryModel(string Author, string Message, string ConversationId, DateTime Date)
{
    public const string DocumentName = "chatHistory";
}

