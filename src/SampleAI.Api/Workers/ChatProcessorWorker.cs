using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.AI;
using SampleAI.Api.Extensions;
using SampleAI.Api.Hubs;
using SampleAI.Application.Services;
using SampleAI.Shared.Models;

namespace SampleAI.Api.Workers;

public class ChatProcessorWorker : BackgroundService
{
    private readonly ChannelReader<ChatMessageRequest> _channelReader;
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<ChatProcessorWorker> _logger;

    public ChatProcessorWorker(
        ChannelReader<ChatMessageRequest> channelReader, 
        IHubContext<ChatHub> hubContext,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<ChatProcessorWorker> logger)
    {
        _channelReader = channelReader;
        _hubContext = hubContext;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

        await foreach (var request in _channelReader.ReadAllAsync(cancellationToken))
        {
            try
            {
                var date = DateTime.Now;
                var conversationId = request.ConversationId.GenerateConversationId();
                await foreach (var contentMessage in chatService.GenerateResponseAsync(request.UserPrompt, conversationId, date, cancellationToken))
                {
                    var response = new ChatHistoryResponse(ChatRole.Assistant.ToString(), contentMessage, conversationId, date);

                    await _hubContext.Clients.Client(request.ConnectionId).SendAsync("ReceiveToken", response, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong");
                await _hubContext.Clients.Client(request.ConnectionId)
                    .SendAsync("Error", "Something went wrong. Please try again later", cancellationToken);
            }
        }
    }
}