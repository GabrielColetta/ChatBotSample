using Microsoft.AspNetCore.SignalR;
using SampleAI.Shared.Models;
using System.Threading.Channels;

namespace SampleAI.Api.Hubs;

public class ChatHub : Hub
{
    private readonly ChannelWriter<ChatMessageRequest> _channelWriter;

    public ChatHub(ChannelWriter<ChatMessageRequest> channelWriter)
    {
        _channelWriter = channelWriter;
    }

    public async Task SendMessageAsync(string userPrompt, Guid? chatId)
    {
        var request = new ChatMessageRequest(Context.ConnectionId, chatId, userPrompt);

        await _channelWriter.WriteAsync(request);
    }
}
