using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Infrastructure.MongoDB.Constants;
using SampleAI.Shared.Constants;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Models;
using System.Linq.Expressions;

namespace SampleAI.Infrastructure.MongoDB.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<Chat> _collection;
    private readonly IMongoCollection<Conversation> _conversationCollection;

    public ChatRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(GlobalVariables.DatabaseName);
        _collection = database.GetCollection<Chat>(DatabaseConstants.ChatCollection);
        _conversationCollection = database.GetCollection<Conversation>(DatabaseConstants.ConversationCollection);
    }

    public async Task CreateAsync(Chat entity, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(entity, null, cancellationToken);

        if (entity.Conversations.Count != 0)
        {
            await _conversationCollection.InsertManyAsync(entity.Conversations, null, cancellationToken);
        }
    }

    public async Task<Chat?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _collection
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<PaginatedResponse<TResponse>> GetPaginatedAsync<TResponse>(
        Expression<Func<Chat, bool>> predicate,
        Expression<Func<Chat, TResponse>> selector,
        PaginateFilter paginateFilter,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        var query = _collection
            .AsQueryable()
            .Where(predicate);

        var totalItems = await query.CountAsync(cancellationToken);

        var response = await query
            .OrderByDescending(x => x.Date)
            .Skip(paginateFilter.CurrentPage * paginateFilter.PerPage)
            .Take(paginateFilter.PerPage)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TResponse>(response, paginateFilter.PerPage, paginateFilter.CurrentPage, totalItems);
    }
}
