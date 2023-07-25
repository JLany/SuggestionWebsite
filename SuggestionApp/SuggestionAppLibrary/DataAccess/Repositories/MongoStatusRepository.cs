using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess.Repositories;

public class MongoStatusRepository : IStatusRepository
{
    private readonly IMongoCollection<StatusModel> _statuses;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Statuses";

    public MongoStatusRepository(IDbConnection dbConnection, IMemoryCache cache)
    {
        _statuses = dbConnection.StatusCollection;
        _cache = cache;
    }

    public async Task<List<StatusModel>> GetStatusesAsync()
    {
        var output = _cache.Get<List<StatusModel>>(CacheKey);

        if (output is null)
        {
            output = (await _statuses.FindAsync(_ => true))
                .ToList();

            _cache.Set(CacheKey, output, TimeSpan.FromDays(1));
        }

        return output;
    }

    public Task CreateStatus(StatusModel status)
    {
        return _statuses.InsertOneAsync(status);
    }
}
