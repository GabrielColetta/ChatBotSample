namespace SampleAI.Api.Extensions;

public static class StringExtensions
{
    public static string GenerateConversationId(this string? conversationId)
    {
        if (string.IsNullOrEmpty(conversationId))
        {
            conversationId = Guid.NewGuid().ToString("N")[..13];
        }
        return conversationId;
    }
}
