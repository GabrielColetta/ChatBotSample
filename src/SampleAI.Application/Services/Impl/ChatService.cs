using Microsoft.Extensions.AI;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;

namespace SampleAI.Application.Services.Impl;

public class ChatService : IChatService
{
    private const int MaxConversationHistorySize = 20;
    private const int CurrentPage = 0;

    private readonly IChatClient _chatClient;
    private readonly IEmbeddingService _embeddingService;
    private readonly IChatRepository _chatRepository;
    private readonly IConversationRepository _conversationRepository;

    public ChatService(
        IChatClient chatClient,
        IEmbeddingService embeddingService,
        IChatRepository chatRepository,
        IConversationRepository conversationRepository)
    {
        _chatClient = chatClient;
        _chatRepository = chatRepository;
        _embeddingService = embeddingService;
        _conversationRepository = conversationRepository;
    }

    public async IAsyncEnumerable<ChatHistoryResponse> GenerateResponseAsync(
        Guid? chatId,
        string content,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chat = await SaveAsync(chatId, content, cancellationToken);

        var chatHistories = await GetChatMessagesAsync(chat.Id, cancellationToken);

        var messageBuilder = new StringBuilder();
        await foreach (var response in _chatClient.GetStreamingResponseAsync(chatHistories, cancellationToken: cancellationToken))
        {
            if (response.Text != null)
            {
                messageBuilder.Append(response.Text);
                yield return new ChatHistoryResponse(ChatRole.Assistant.ToString(), response.Text, chat.Id.ToString(), chat.Date);
            }
        }

        if (messageBuilder.Length > 0)
        {
            await UpdateAsync(chat, ChatRole.Assistant, messageBuilder.ToString(), cancellationToken);
        }
    }

    private async Task<Chat> SaveAsync(Guid? chatId, string content, CancellationToken cancellationToken)
    {
        if (chatId.HasValue)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId.Value, cancellationToken);
            if (chat != null)
            {
                await UpdateAsync(chat, ChatRole.User, content, cancellationToken);

                return chat;
            }
        }

        return await CreateAsync(content, cancellationToken);
    }

    private async Task<Chat> CreateAsync(string message, CancellationToken cancellationToken)
    {
        var embedding = await _embeddingService.GetEmbeddingFromModelAsync(message, cancellationToken);

        var chat = new Chat(message, ChatRole.User.Value, message, embedding);

        await _chatRepository.CreateAsync(chat, cancellationToken);

        return chat;
    }

    private async Task UpdateAsync(Chat chat, ChatRole chatRole, string content, CancellationToken cancellationToken)
    {
        var embedding = await _embeddingService.GetEmbeddingFromModelAsync(content, cancellationToken);

        var conversation = new Conversation(chat.Id, chatRole.Value, DateTime.UtcNow, content, embedding);

        await _conversationRepository.CreateAsync(conversation, cancellationToken);
    }

    private async Task<IList<ChatMessage>> GetChatMessagesAsync(Guid chatId, CancellationToken cancellationToken)
    {
        var paginateFilter = new PaginateFilter(MaxConversationHistorySize, CurrentPage);

        var response = await _conversationRepository.GetPaginatedAsync(
            GetPredicate(chatId),
            paginateFilter,
            cancellationToken);

        return [.. response.Data
            .OrderBy(x => x.Date)
            .ThenBy(x => x.Id)
            .Select(x => new ChatMessage(new ChatRole(x.ChatRole.ToLowerInvariant()), x.Content))];
    }

    private static Expression<Func<Conversation, bool>> GetPredicate(Guid chatId)
    {
        return x => x.ChatId == chatId;
    }
}
