namespace SuggestionAppLibrary.DataAccess.Repositories;

public class MongoUserRepository : IUserRepository
{
    private readonly IMongoCollection<UserModel> _users;
    //private readonly IMemoryCache _cache;
    //private const string CacheKey = "Users";

    public MongoUserRepository(IDbConnection dbConnection)
    {
        _users = dbConnection.UserCollection;
    }

    public async Task<List<UserModel>> GetUsersAsync()
    {
        //var output = _cache.Get<List<UserModel>>(CacheKey);

        //if (output is null)
        //{
        //    output = (await _users.FindAsync(_ => true)).ToList();

        //    _cache.Set(CacheKey, output, TimeSpan.FromDays(1));
        //}

        //return output;

        var results = await _users.FindAsync(_ => true);
        return results.ToList();
    }

    public async Task<UserModel> GetUserAsync(string id)
    {
        var results = await _users.FindAsync(u => u.Id == id);
        return results.FirstOrDefault();
    }

    public async Task<UserModel> GetUserFromAuthenticationAsync(string objectId)
    {
        var results = await _users.FindAsync(u => u.ObjectIdentifier == objectId);
        return results.FirstOrDefault();
    }

    public async Task CreateUserAsync(UserModel user)
    {
        await _users.InsertOneAsync(user);
    }

    public async Task UpdateUserAsync(UserModel user)
    {
        var filter = Builders<UserModel>.Filter.Eq("Id", user.Id);
        await _users.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = true });
    }
}
