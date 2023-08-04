namespace SuggestionAppLibrary.DataAccess.Repositories
{
    public interface IPostRepository
    {
        Task CreatePostAsync(PostModel post);
        Task<PostModel> GetPostAsync(string id);
        Task<List<PostModel>> GetPostsAsync();
        Task<List<PostModel>> GetPostsAsync(ApprovalStatus approvalStatus);
		Task<List<PostModel>> GetPostsAsync(string userId);
		Task UpdatePostAsync(PostModel post);
        Task UpvotePostAsync(string postId, string userId);
    }
}