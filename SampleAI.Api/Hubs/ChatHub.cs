using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;
using System.Text;

namespace SampleAI.Api.Hubs;

public class ChatHub : Hub<IChatClient>
{
    private readonly ILogger<ChatHub> _logger;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly IDatabaseContext _databaseContext;

    public ChatHub(IChatCompletionService chatCompletionService, IDatabaseContext databaseContext, ILogger<ChatHub> logger)
    {
        _chatCompletionService = chatCompletionService;
        _databaseContext = databaseContext;
        _logger = logger;
    }

    public async Task SendMessageAsync(string message, string conversationId)
    {
        await Clients.All.ReceiveMessageAsync(AuthorRole.User.ToString(), message, conversationId);

        var chatHistory = await GetChatHistoryAsync(message, conversationId);
        await GenerateStreamingBotResponse(chatHistory, conversationId);
    }

    private async Task<string> GenerateStreamingBotResponse(ChatHistory chatHistory, string conversationId)
    {
        var timestamp = DateTime.Now;

        var buffer = new StringBuilder();
        var message = new StringBuilder();
        try
        {
            await foreach (var response in _chatCompletionService.GetStreamingChatMessageContentsAsync(chatHistory))
            {
                if (response?.Content != null)
                {
                    buffer.Append(response.Content);

                    if (response.Content.Contains('\n'))
                    {
                        var contentToSend = buffer.ToString();
                        await Clients.Caller.ReceiveMessageAsync(AuthorRole.Assistant.ToString(), contentToSend, conversationId);
                        message.Append(contentToSend);
                        await _databaseContext.InsertAsync(ChatHistoryModel.DocumentName, new ChatHistoryModel(AuthorRole.Assistant.ToString(), message.ToString(), conversationId, timestamp));
                        buffer.Clear();
                    }
                }
            }

            if (buffer.Length > 0)
            {
                var remainingContent = buffer.ToString();
                await Clients.All.ReceiveMessageAsync(AuthorRole.Assistant.ToString(), remainingContent, conversationId);
                message.Append(remainingContent);
                await _databaseContext.InsertAsync(ChatHistoryModel.DocumentName, new ChatHistoryModel(AuthorRole.Assistant.ToString(), message.ToString(), conversationId, timestamp));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            message.Append("An error occurred while processing your request.");
            await Clients.Caller.ReceiveMessageAsync(AuthorRole.System.ToString(), "Something went wrong, see logs pls.", conversationId);
        }
        return message.ToString();
    }

    private async Task<ChatHistory> GetChatHistoryAsync(string newMessage, string conversationId)
    {
        var filter = GeChatFilter();
        var response = await _databaseContext.GetPaginatedAsync(ChatHistoryModel.DocumentName, filter);
        var messageContent = response.Select(x => new ChatMessageContent(new AuthorRole(x.Author), x.Message));

        if (messageContent.Any())
        {
            await Clients.Caller.ReceiveMessageAsync(AuthorRole.Assistant.ToString(), "Hello there, it looks like we already called before, would you like to continue to talk?", conversationId);
            var userMessage = await Clients.Caller.GetMessageAsync();

            // TODO: Add sentiment anaylysys to tell if it's positive or negative
            if (string.IsNullOrWhiteSpace(userMessage) || userMessage.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                var chatHistory = new ChatHistory(messageContent);
                chatHistory.AddUserMessage(newMessage);

                return chatHistory;
            }
        }
        return new ChatHistory(newMessage);
    }

    private static PaginateFilters<ChatHistoryModel> GeChatFilter()
    {
        return new PaginateFilters<ChatHistoryModel>(PerPage: 5, x => x.Date, x => x.ConversationId);
    }
}
