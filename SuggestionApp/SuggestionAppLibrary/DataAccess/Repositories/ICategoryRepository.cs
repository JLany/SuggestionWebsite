namespace SuggestionAppLibrary.DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        Task CreateCategory(CategoryModel category);
        Task<List<CategoryModel>> GetCategoriesAsync();
    }
}