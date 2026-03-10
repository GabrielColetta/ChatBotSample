#pragma warning disable CS8618

namespace SampleAI.Domain.Entities;

public class Chat
{
    public Guid Id { get; init; }
    public string Title { get; init; }
    public DateTime Date { get; init; }

    public ICollection<Conversation> Conversations { get; init; } = [];

    private Chat()
    {
    }

    public Chat(string title, string chatRole, string content, float[] contentEmbedding)
    {
        Id = Guid.CreateVersion7();
        Title = title;

        var dt = DateTime.UtcNow;
        Date = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, DateTimeKind.Utc);

        AddConversation(chatRole, Date, content, contentEmbedding);
    }

    public void AddConversation(string chatRole, DateTime date, string content, float[] contentEmbedding)
    {
        Conversations.Add(new Conversation(Id, chatRole, date, content, contentEmbedding));
    }
}
