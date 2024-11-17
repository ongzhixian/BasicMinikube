using MongoDB.Bson;
using MongoDB.Driver;
using SimpleWebApi.DataEntities;

namespace SimpleWebApi.Services;

public class MongoDbServices
{
    private const string databaseName = "hci";

    internal static void AddDefinitions(IServiceCollection services, ConfigurationManager configuration)
    {
        // See: https://learn.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-8.0

        services.AddSingleton<IMongoClient, MongoClient>(_ =>
        {
            string mongoDbConnectionString = configuration["ConnectionStrings:HciMongoDb"] ?? throw new NullReferenceException("ConnectionStrings:HciMongoDb configuration null.");
            return new MongoClient(mongoDbConnectionString);
        });

        services.AddKeyedSingleton(databaseName, (sp, key) =>
        {
            var mongoClient = sp.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase((string)key);
        });

        services.AddKeyedSingleton("users", (sp, key) =>
        {
            var database = sp.GetRequiredKeyedService<IMongoDatabase>(databaseName);
            return database.GetCollection<BsonDocument>((string)key);
        });

        services.AddKeyedSingleton("course", (sp, key) =>
        {
            var database = sp.GetRequiredKeyedService<IMongoDatabase>(databaseName);
            return database.GetCollection<Course>((string)key);
        });

    }
}
