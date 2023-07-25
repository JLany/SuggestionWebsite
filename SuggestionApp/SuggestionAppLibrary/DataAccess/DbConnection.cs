using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess;

public class DbConnection : IDbConnection
{
    private readonly IConfiguration _config;
    private readonly IMongoDatabase _database;
    private const string ConnectionName = "MongoDB";

    public string DbName { get; private set; }

    public string CategoryCollectionName { get; } = "categories";
    public string StatusCollectionName { get; } = "statuses";
    public string UserCollectionName { get; } = "users";
    public string PostCollectionName { get; } = "posts";

    public IMongoClient Client { get; private set; }
    public IMongoCollection<CategoryModel> CategoryCollection { get; private set; }
    public IMongoCollection<StatusModel> StatusCollection { get; private set; }
    public IMongoCollection<UserModel> UserCollection { get; private set; }
    public IMongoCollection<PostModel> PostCollection { get; private set; }

    public DbConnection(IConfiguration config)
    {
        _config = config;

        Client = new MongoClient(_config.GetConnectionString(ConnectionName));

        DbName = _config["DatabaseName"];
        _database = Client.GetDatabase(DbName);

        CategoryCollection = _database.GetCollection<CategoryModel>(CategoryCollectionName);
        StatusCollection = _database.GetCollection<StatusModel>(StatusCollectionName);
        UserCollection = _database.GetCollection<UserModel>(UserCollectionName);
        PostCollection = _database.GetCollection<PostModel>(PostCollectionName);
    }
}
