using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using SampleAI.Api.Extensions;
using SampleAI.Application.Services;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Hubs;

public class ChatHub : Hub<IChatHubClient>
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async IAsyncEnumerable<ChatHistoryResponse> SendMessageAsync(string message, string? conversationId)
    {
        var date = DateTime.Now;
        conversationId = conversationId.GenerateConversationId();
        await foreach (var contentMessage in _chatService.GenerateResponseAsync(message, conversationId!, date))
        {
            yield return new ChatHistoryResponse(ChatRole.Assistant.ToString(), contentMessage, conversationId, date);
        }
    }
}
