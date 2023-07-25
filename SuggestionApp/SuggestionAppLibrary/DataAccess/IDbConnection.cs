using MongoDB.Driver;

namespace SuggestionAppLibrary.DataAccess
{
    public interface IDbConnection
    {
        IMongoCollection<CategoryModel> CategoryCollection { get; }
        IMongoClient Client { get; }
        string DbName { get; }
        IMongoCollection<PostModel> PostCollection { get; }
        IMongoCollection<StatusModel> StatusCollection { get; }
        IMongoCollection<UserModel> UserCollection { get; }
    }
}