#pragma warning disable CS8618

namespace SampleAI.Domain.Entities;

public class Conversation
{
    public Guid Id { get; init; }
    public string ChatRole { get; init; }
    public DateTime Date { get; init; }
    public string Content { get; init; }
    public float[] ContentEmbedding { get; init; }
    public double? Score { get; set; }

    public Guid ChatId { get; init; }
    public Chat Chat { get; init; }

    public Conversation()
    {
        
    }

    public Conversation(Guid chatId, string chatRole, DateTime date, string content, float[] contentEmbedding)
    {
        Id = Guid.CreateVersion7();
        ChatId = chatId;    
        Date = date;
        ChatRole = chatRole;
        Content = content;
        ContentEmbedding = contentEmbedding;
    }
}
