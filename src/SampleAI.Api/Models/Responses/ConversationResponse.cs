using SampleAI.Application.Contracts.Responses;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Models.Responses;

public class ConversationResponse
{
    public ConversationResponse(string chatId, string chatRole, DateTime date, string content)
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

    public static PaginatedResponse<ConversationResponse> From(PaginatedResponse<GetConversationByIdResponse> paginatedResponse)
    {
        var chatHistoryModel = paginatedResponse.Data.Select(x => new ConversationResponse(x.Id.ToString(), x.ChatRole, x.Date, x.Content));
        return new PaginatedResponse<ConversationResponse>(chatHistoryModel, paginatedResponse.PerPage, paginatedResponse.CurrentPage, paginatedResponse.TotalItems);
    }
}
