using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using SampleAI.Application.Services;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Api.Hubs;

public class ChatHub : Hub<IChatHubClient>
{
    private readonly IChatService _chatService;

    public ChatHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task SendMessageAsync(string message, string conversationId)
    {
        await foreach (var contentMessage in _chatService.GenerateResponseAsync(message, conversationId))
        {
            await Clients.Caller.ReceiveMessageAsync(ChatRole.Assistant.ToString(), contentMessage, conversationId);
        };
    }
}
