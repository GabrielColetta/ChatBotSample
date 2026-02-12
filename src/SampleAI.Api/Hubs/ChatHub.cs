using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using SampleAI.Api.Extensions;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ChannelWriter<ChatMessageRequest> _channelWriter;

    public ChatHub(ChannelWriter<ChatMessageRequest> channelWriter)
    {
        _channelWriter = channelWriter;
    }

    public async Task SendMessageAsync(string userPrompt, string? conversationId)
    {
        var request = new ChatMessageRequest(Context.ConnectionId, conversationId.GenerateConversationId(), userPrompt);
        
        await _channelWriter.WriteAsync(request);
    }
}
