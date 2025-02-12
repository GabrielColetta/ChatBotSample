using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SampleAI.Shared.Constants;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;
using SampleAI.Shared.Models;

namespace SampleAI.Infrastructure.MongoDB.Context;

public class MongoContext : IDatabaseContext
{
    internal IMongoDatabase Database { get; private set; }

    public MongoContext(IMongoClient client)
    {
        Database = client.GetDatabase(GlobalVariables.InstanceName);
        var pack = new ConventionPack
        {
            new IgnoreExtraElementsConvention(true)
        };
        ConventionRegistry.Register(nameof(MongoContext), pack, t => true);
    }

    public async Task InsertAsync<TDocument>(string name, TDocument document, CancellationToken cancellationToken)
        where TDocument : class
    {
        var collection = Database.GetCollection<TDocument>(name);

        await collection.InsertOneAsync(document, new InsertOneOptions(), cancellationToken);
    }

    public async Task<IEnumerable<TDocument>> GetPaginatedAsync<TDocument>(string name, PaginateFilters<TDocument> paginateFilter, CancellationToken cancellationToken)
        where TDocument : class
    {
        var collection = Database.GetCollection<TDocument>(name);

        var queryable = collection
            .AsQueryable()
            .Skip(paginateFilter.CurrentPage * paginateFilter.PerPage)
            .Take(paginateFilter.PerPage);

        if (paginateFilter.FilterBy != null)
        {
            queryable = queryable.Where(paginateFilter.FilterBy);
        }

        return await queryable.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<TDocument>> GetSamplePaginatedAsync<TDocument>(
        string name, 
        PaginateFilters<TDocument> paginateFilter, 
        CancellationToken cancellationToken)
        where TDocument : class
    {
        var collection = Database.GetCollection<TDocument>(name);

        var queryable = collection
            .AsQueryable()
            .Skip(paginateFilter.CurrentPage * paginateFilter.PerPage)
            .Take(paginateFilter.PerPage);

        if (paginateFilter.FilterBy != null)
        {
            queryable = queryable.Where(paginateFilter.FilterBy);
        }
        if (paginateFilter.SortByDesc != null)
        {
            queryable = queryable.OrderByDescending(paginateFilter.SortByDesc);
        }

        if (paginateFilter.GroupBy != null)
        {
            queryable = queryable.GroupBy(paginateFilter.GroupBy).Select(x => x.First());
        }

        var totalItems = await queryable.CountAsync(cancellationToken);

        var data = await queryable.ToListAsync(cancellationToken);

        return new PaginatedResponse<TDocument>(data, paginateFilter.PerPage, paginateFilter.CurrentPage, (uint)totalItems);
    }
}