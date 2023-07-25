namespace SuggestionAppLibrary.DataAccess.Repositories
{
    public interface IUserRepository
    {
        Task CreateUserAsync(UserModel user);
        Task<UserModel> GetUserAsync(string id);
        Task<UserModel> GetUserFromAuthenticationAsync(string objectId);
        Task<List<UserModel>> GetUsersAsync();
        Task UpdateUserAsync(UserModel user);
    }
}