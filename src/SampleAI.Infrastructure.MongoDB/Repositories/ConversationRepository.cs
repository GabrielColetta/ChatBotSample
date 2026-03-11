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

public class ConversationRepository : IConversationRepository
{
    private const double MinScore = 0.7;

    private readonly IMongoCollection<Conversation> _collection;

    public ConversationRepository(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase(GlobalVariables.DatabaseName);
        _collection = database.GetCollection<Conversation>(DatabaseConstants.ConversationCollection);
    }

    public async Task CreateAsync(Conversation entity, CancellationToken cancellationToken)
    {
        await _collection.InsertOneAsync(entity, null, cancellationToken);
    }

    public async Task<PaginatedResponse<TResponse>> GetPaginatedAsync<TResponse>(
        Expression<Func<Conversation, bool>> predicate,
        Expression<Func<Conversation, TResponse>> selector,
        PaginateFilter paginateFilter,
        CancellationToken cancellationToken)
        where TResponse : class
    {
        var query = _collection
            .AsQueryable()
            .Where(predicate);

        var totalItems = await query.CountAsync(cancellationToken);

        var response = await query
            .OrderBy(x => x.Date)
            .Skip(paginateFilter.CurrentPage * paginateFilter.PerPage)
            .Take(paginateFilter.PerPage)
            .Select(selector)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<TResponse>(response, paginateFilter.PerPage, paginateFilter.CurrentPage, totalItems);
    }

    public async Task<PaginatedResponse<Conversation>> VectorSearchAsync(
        PaginateFilter paginateFilter,
        float[] embedding,
        CancellationToken cancellationToken)
    {
        double[] queryVector = Array.ConvertAll(embedding, x => (double)x);

        var projection = Builders<Conversation>.Projection
            .Include(x => x.Date)
            .Include(x => x.Content)
            .Include(x => x.ChatId)
            .MetaVectorSearchScore(x => x.Score);

        var response = await _collection
            .Aggregate()
            .VectorSearch(
                x => x.ContentEmbedding,
                queryVector,
                paginateFilter.PerPage,
                new VectorSearchOptions<Conversation>
                {
                    IndexName = DatabaseConstants.EmbeddingsIndexName,
                    NumberOfCandidates = paginateFilter.PerPage * 10
                }
            )
            .Project<Conversation>(projection)
            .Match(c => c.Score >= MinScore)
            .ToListAsync(cancellationToken);

        return new PaginatedResponse<Conversation>(response, paginateFilter.PerPage, paginateFilter.CurrentPage, response.Count);
    }
}
