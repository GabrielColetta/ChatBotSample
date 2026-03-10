using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SampleAI.Domain.Entities;
using SampleAI.Domain.Interfaces;
using SampleAI.Infrastructure.MongoDB.Constants;
using SampleAI.Infrastructure.MongoDB.Repositories;
using SampleAI.Infrastructure.MongoDB.Services;
using SampleAI.Shared.Constants;
using SampleAI.Shared.Interfaces;

namespace SampleAI.Infrastructure.MongoDB.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterMappings();

        var connectionString = configuration.GetConnectionString(GlobalVariables.DatabaseName)!;

        services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IDatabaseInitializer, MongoDbIndexInitializer>();

        return services;
    }

    private static void RegisterMappings()
    {
        BsonSerializer.TryRegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        var pack = new ConventionPack {
            new CamelCaseElementNameConvention(),
            new IgnoreExtraElementsConvention(true)
        };

        ConventionRegistry.Register("DefaultConvention", pack, t => true);

        if (!BsonClassMap.IsClassMapRegistered(typeof(Chat)))
        {
            BsonClassMap.TryRegisterClassMap<Chat>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                cm.MapProperty(c => c.Title).SetElementName("title");
                cm.MapProperty(c => c.Date).SetElementName("date");
                cm.UnmapProperty(c => c.Conversations);
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Conversation)))
        {
            BsonClassMap.TryRegisterClassMap<Conversation>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(c => c.Id);
                cm.MapProperty(c => c.ChatRole).SetElementName("role");
                cm.MapProperty(c => c.Date).SetElementName("date");
                cm.MapProperty(c => c.Content).SetElementName("content");

                cm.MapProperty(c => c.ContentEmbedding)
                    .SetElementName(DatabaseConstants.ContentEmbedding)
                    .SetSerializer(new ArraySerializer<float>(new SingleSerializer(BsonType.Double)));

                cm.UnmapProperty(c => c.Chat);
                cm.MapProperty(c => c.Score)
                    .SetElementName("score")
                    .SetIgnoreIfNull(true);
            });
        }
    }
}
