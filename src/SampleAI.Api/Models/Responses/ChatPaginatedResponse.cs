using SampleAI.Domain.Entities;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Models.Responses;

public class ChatPaginatedResponse
{
    public ChatPaginatedResponse(string chatId, string title, DateTime date)
    {
        ChatId = chatId;
        Title = title;
        Date = date;
    }

    public string ChatId { get; init; }
    public string Title { get; init; }
    public DateTime Date { get; init; }

    public static PaginatedResponse<ChatPaginatedResponse> From(PaginatedResponse<Chat> paginatedResponse)
    {
        var chatHistoryModel = paginatedResponse.Data.Select(x => new ChatPaginatedResponse(x.Id.ToString(), x.Title, x.Date));
        return new PaginatedResponse<ChatPaginatedResponse>(chatHistoryModel, paginatedResponse.PerPage, paginatedResponse.CurrentPage, paginatedResponse.TotalItems);
    }
}
