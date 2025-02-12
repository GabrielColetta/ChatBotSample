using SampleAI.Shared.Models;

namespace SampleAI.Api.Models.Responses;

public class HistoryResponse
{
    public HistoryResponse(string conversationId, string chatRole, DateTime date, string content)
    {
        ConversationId = conversationId;
        ChatRole = chatRole;
        Date = date;
        Content = content;
    }

    public string ConversationId { get; private set; }
    public string ChatRole { get; private set; }
    public DateTime Date { get; private set; }
    public string Content { get; private set; }

    public static PaginatedResponse<HistoryResponse> From(PaginatedResponse<ChatHistoryModel> paginatedResponse)
    {
        var chatHistoryModel = paginatedResponse.Data.Select(x => new HistoryResponse(x.ConversationId, x.ChatRole, x.Date, x.Content));
        return new PaginatedResponse<HistoryResponse>(chatHistoryModel, paginatedResponse.PerPage, paginatedResponse.CurrentPage, paginatedResponse.TotalItems);
    }

    public static IEnumerable<HistoryResponse> From(IEnumerable<ChatHistoryModel> chatHistories)
    {
        return chatHistories.Select(x => new HistoryResponse(x.ConversationId, x.ChatRole, x.Date, x.Content));
    }
}
