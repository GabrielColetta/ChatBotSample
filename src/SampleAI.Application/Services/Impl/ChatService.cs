using Microsoft.Extensions.AI;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;
using System.Text;

namespace SampleAI.Application.Services.Impl;

public class ChatService : IChatService
{
    private readonly IChatClient _chatClient;
    private readonly IDatabaseContext _databaseContext;

    public ChatService(IChatClient chatClient, IDatabaseContext databaseContext)
    {
        _chatClient = chatClient;
        _databaseContext = databaseContext;
    }

    public async IAsyncEnumerable<string> GenerateResponseAsync(string message, string conversationId)
    {
        var chatHistory = await GetChatMessagesAsync(message, conversationId);
        var timestamp = DateTime.Now;

        var messageBuilder = new StringBuilder();
        await foreach (var response in _chatClient.CompleteStreamingAsync(chatHistory))
        {
            if (response?.Contents != null)
            {
                messageBuilder.Append(response.Text);
                yield return messageBuilder.ToString();
            }
        }

        if (messageBuilder.Length > 0)
        {
            await UpdateChatHistoryAsync(conversationId, timestamp, messageBuilder);
        }
    }

    private async Task UpdateChatHistoryAsync(string conversationId, DateTime timestamp, StringBuilder message)
    {
        var chatHistoryModel = new ChatHistoryModel(ChatRole.Assistant.ToString(), message.ToString(), conversationId, timestamp);
        await _databaseContext.InsertAsync(ChatHistoryModel.DocumentName, chatHistoryModel);
    }

    private async Task<IList<ChatMessage>> GetChatMessagesAsync(string content, string conversationId)
    {
        var newMessage = new ChatMessage(ChatRole.User, content);

        var response = await _databaseContext.GetPaginatedAsync(ChatHistoryModel.DocumentName, GeChatFilter());
        var messageContent = response.Select(x => new ChatMessage(new ChatRole(x.ChatRole), x.Message)).ToList();

        if (messageContent.Count != 0)
        {
            messageContent.Add(newMessage);
            return messageContent;
        }

        return [newMessage];
    }

    private static PaginateFilters<ChatHistoryModel> GeChatFilter()
    {
        return new PaginateFilters<ChatHistoryModel>(PerPage: 5, x => x.Date, x => x.ConversationId);
    }
}
