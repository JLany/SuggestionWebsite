using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess.Repositories;

public class MongoPostRepository : IPostRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<PostModel> _posts;
    private const string CacheKey = "Posts";

    public MongoPostRepository(IDbConnection dbConnection, IUserRepository userRepository, IMemoryCache cache)
    {
        _dbConnection = dbConnection;
        _userRepository = userRepository;
        _cache = cache;
        _posts = _dbConnection.PostCollection;
    }

    public async Task<List<PostModel>> GetPostsAsync()
    {
        var output = _cache.Get<List<PostModel>>(CacheKey);

        if (output is null)
        {
            output = (await _posts
                .FindAsync(p => p.ApprovalStatus != ApprovalStatus.Archived))
                .ToList();

            _cache.Set(CacheKey, output, TimeSpan.FromMinutes(1));
        }

        return output;
    }

    public async Task<List<PostModel>> GetPostsAsync(ApprovalStatus approvalStatus)
    {
        return (await GetPostsAsync())
            .Where(p => p.ApprovalStatus == approvalStatus)
            .ToList();
    }

    public async Task<PostModel> GetPostAsync(string id)
    {
        var results = await _posts.FindAsync(p => p.Id == id);
        return results.FirstOrDefault();
    }

    public async Task UpdatePostAsync(PostModel post)
    {
        await _posts.ReplaceOneAsync(p => p.Id == post.Id, post);
        _cache.Remove(CacheKey);
    }

    public async Task UpvotePostAsync(string postId, string userId)
    {
        using IClientSessionHandle session = await _dbConnection.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            PostModel post = await GetPostAsync(postId);
            UserModel user = await _userRepository.GetUserAsync(userId);

            bool isUpvote = post.UserVotes.Add(userId);

            if (!isUpvote)
            {
                post.UserVotes.Remove(userId);

                user.VotedOnPosts
                    .Remove(
                    user.VotedOnPosts
                    .Where(p => p.Id == postId)
                    .First()
                    );
            }
            else
            {
                user.VotedOnPosts.Add(new UserPostViewModel(post));
            }

            await UpdatePostAsync(post);
            await _userRepository.UpdateUserAsync(user);

            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }

    public async Task CreatePostAsync(PostModel post)
    {
        using IClientSession session = await _dbConnection.Client.StartSessionAsync();
        session.StartTransaction();

        try
        {
            UserModel user = await _userRepository.GetUserAsync(post.Author.Id);

            await _posts.InsertOneAsync(post);

            user.AuthoredPosts.Add(new UserPostViewModel(post));

            await _userRepository.UpdateUserAsync(user);

            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
