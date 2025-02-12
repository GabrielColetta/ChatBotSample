using Microsoft.Extensions.AI;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;
using System.Text;

namespace SampleAI.Application.Services.Impl;

public class ChatService : IChatService
{
    private const int MaxConversationHistorySize = 20;
    private const int CurrentPage = 0;

    private readonly IChatClient _chatClient;
    private readonly IDatabaseContext _databaseContext;

    public ChatService(IChatClient chatClient, IDatabaseContext databaseContext)
    {
        _chatClient = chatClient;
        _databaseContext = databaseContext;
    }

    public async IAsyncEnumerable<string> GenerateResponseAsync(string message, string conversationId)
    {
        var timestamp = DateTime.Now;

        await UpdateChatHistoryAsync(ChatRole.User, conversationId, timestamp, message);
        var chatHistories = await GetChatMessagesAsync(conversationId);

        var messageBuilder = new StringBuilder();
        await foreach (var response in _chatClient.CompleteStreamingAsync(chatHistories))
        {
            if (response?.Contents != null)
            {
                messageBuilder.Append(response.Text);
                yield return messageBuilder.ToString();
            }
        }

        if (messageBuilder.Length > 0)
        {
            await UpdateChatHistoryAsync(ChatRole.Assistant, conversationId, timestamp, messageBuilder.ToString());
        }
    }

    private async Task UpdateChatHistoryAsync(ChatRole chatRole, string conversationId, DateTime timestamp, string message)
    {
        var chatHistoryModel = new ChatHistoryModel(chatRole.ToString(), message, conversationId, timestamp);
        await _databaseContext.InsertAsync(ChatHistoryModel.DocumentName, chatHistoryModel, CancellationToken.None);
    }

    private async Task<IList<ChatMessage>> GetChatMessagesAsync(string conversationId)
    {
        var response = await _databaseContext.GetPaginatedAsync(ChatHistoryModel.DocumentName, GeChatFilter(conversationId), CancellationToken.None);
        return response.Select(x => new ChatMessage(new ChatRole(x.ChatRole), x.Content)).ToList();
    }

    private static PaginateFilters<ChatHistoryModel> GeChatFilter(string conversationId)
    {
        return new PaginateFilters<ChatHistoryModel>(MaxConversationHistorySize, CurrentPage, sortByDesc: x => x.Date, filterBy: x => x.ConversationId == conversationId);
    }
}
