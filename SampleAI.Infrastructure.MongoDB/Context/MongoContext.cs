using MongoDB.Driver;
using SampleAI.Shared.Constants;
using SampleAI.Shared.Filters;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Infrastructure.MongoDB.Context;

public class MongoContext : IDatabaseContext
{
    internal IMongoDatabase Database { get; private set; }

    public MongoContext(IMongoClient client)
    {
        Database = client.GetDatabase(GlobalVariables.InstanceName);
    }

    public async Task InsertAsync<TDocument>(string name, TDocument document)
        where TDocument : class
    {
        var collection = Database.GetCollection<TDocument>(name);

        await collection.InsertOneAsync(document);
    }

    public async Task<IEnumerable<TDocument>> GetPaginatedAsync<TDocument>(string name, PaginateFilters<TDocument> paginateFilter)
        where TDocument : class
    {
        var collection = Database.GetCollection<TDocument>(name);

        var filter = Builders<TDocument>.Filter.Empty;
        var sort = Builders<TDocument>.Sort.Descending(paginateFilter.SortByDesc);

        var response = await collection
            .Aggregate()
            .Sort(sort)
            .Limit(paginateFilter.PerPage)
            .Group(paginateFilter.GroupBy, x => x.ToList())
            .ToListAsync();

        return response.SelectMany(x => x);
    }
}