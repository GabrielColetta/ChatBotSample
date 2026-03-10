using SampleAI.Domain.Entities;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Models.Responses;

public class HistoryResponse
{
    public HistoryResponse(string chatId, string chatRole, DateTime date, string content)
    {
        ChatId = chatId;
        ChatRole = chatRole;
        Date = date;
        Content = content;
    }

    public string ChatId { get; private set; }
    public string ChatRole { get; private set; }
    public DateTime Date { get; private set; }
    public string Content { get; private set; }

    public static PaginatedResponse<HistoryResponse> From(PaginatedResponse<Conversation> paginatedResponse)
    {
        var chatHistoryModel = paginatedResponse.Data.Select(x => new HistoryResponse(x.ChatId.ToString(), x.ChatRole, x.Date, x.Content));
        return new PaginatedResponse<HistoryResponse>(chatHistoryModel, paginatedResponse.PerPage, paginatedResponse.CurrentPage, paginatedResponse.TotalItems);
    }
}
