using MongoDB.Bson;
using MongoDB.Driver;
using SampleAI.Domain.Entities;
using SampleAI.Infrastructure.MongoDB.Constants;
using SampleAI.Shared.Constants;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Infrastructure.MongoDB.Services;

public class MongoDbIndexInitializer : IDatabaseInitializer
{
    private readonly IMongoDatabase _database;
    private readonly IMongoCollection<Conversation> _collection;

    public MongoDbIndexInitializer(IMongoClient client)
    {
        _database = client.GetDatabase(GlobalVariables.DatabaseName);
        _collection = _database.GetCollection<Conversation>("conversations");
    }

    /// <summary>
    /// Configure the MongoDB collections and array. 
    /// If you change the embedding model you'll need to change the numDimensions and similarity.
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        using var cursor = await _collection.SearchIndexes.ListAsync();
        var existingIndexes = await cursor.ToListAsync();

        var collections = await (await _database.ListCollectionNamesAsync()).ToListAsync();

        if (!collections.Contains(DatabaseConstants.ChatCollection))
        {
            await _database.CreateCollectionAsync(DatabaseConstants.ChatCollection);
        }

        if (!collections.Contains(DatabaseConstants.ConversationCollection))
        {
            await _database.CreateCollectionAsync(DatabaseConstants.ConversationCollection);
        }

        await Task.Delay(3000);

        bool indexExists = existingIndexes.Any(x => x.GetValue("name") == DatabaseConstants.EmbeddingsIndexName);

        if (!indexExists)
        {
            var indexModel = new CreateSearchIndexModel(
                DatabaseConstants.EmbeddingsIndexName,
                SearchIndexType.VectorSearch,
                new BsonDocument("fields", new BsonArray
                {
                    new BsonDocument
                    {
                        { "type", "vector" },
                        { "path", DatabaseConstants.ContentEmbedding },
                        { "numDimensions", 384 },
                        { "similarity", "cosine" }
                    }
                })
            );

            await _collection.SearchIndexes.CreateOneAsync(indexModel);
        }
    }
}
