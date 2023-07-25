using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess.Repositories;

public class MongoCategoryRepository : ICategoryRepository
{
    private readonly IMongoCollection<CategoryModel> _categories;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Categories";

    public MongoCategoryRepository(IDbConnection dbConnection, IMemoryCache cache)
    {
        _categories = dbConnection.CategoryCollection;
        _cache = cache;
    }

    public async Task<List<CategoryModel>> GetCategoriesAsync()
    {
        var output = _cache.Get<List<CategoryModel>>(CacheKey);

        if (output is null)
        {
            output = (await _categories.FindAsync(_ => true))
                .ToList();

            _cache.Set(CacheKey, output, TimeSpan.FromDays(1));
        }

        return output;
    }

    public Task CreateCategory(CategoryModel category)
    {
        return _categories.InsertOneAsync(category);
    }
}
