namespace SuggestionAppLibrary.DataAccess.Repositories
{
    public interface IStatusRepository
    {
        Task CreateStatus(StatusModel status);
        Task<List<StatusModel>> GetStatusesAsync();
    }
}