﻿using Microsoft.Extensions.Caching.Memory;

namespace SuggestionAppLibrary.DataAccess.Repositories;

public class MongoPostRepository : IPostRepository
{
    private readonly IDbConnection _dbConnection;
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _cache;
    private readonly IMongoCollection<PostModel> _posts;
    private const string CacheKey = "Posts";

    public MongoPostRepository(IDbConnection dbConnection
        , IUserRepository userRepository, IMemoryCache cache)
    {
        _dbConnection = dbConnection;
        _userRepository = userRepository;
        _cache = cache;
        _posts = _dbConnection.PostCollection;
    }

    /// <summary>
    /// Get all posts.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Get all posts with the specified <see cref="ApprovalStatus"/>.
    /// </summary>
    /// <param name="approvalStatus"></param>
    /// <returns></returns>
    public async Task<List<PostModel>> GetPostsAsync(ApprovalStatus approvalStatus)
    {
        return (await GetPostsAsync())
            .Where(p => p.ApprovalStatus == approvalStatus)
            .ToList();
    }

    /// <summary>
    /// Get all posts for a the <see cref="UserModel"/> with the specified Id.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<List<PostModel>> GetPostsAsync(string userId)
    {
        var output = _cache.Get<List<PostModel>>(userId);

        if (output is null)
        {
            var result = await _posts.FindAsync(p => p.Author.Id == userId);
            output = result.ToList();

            _cache.Set(userId, output, TimeSpan.FromMinutes(5));
        }

        return output;
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
        _cache.Remove(post.Author.Id);
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

            _cache.Remove(post.Author.Id);

            await session.CommitTransactionAsync();
        }
        catch (Exception)
        {
            await session.AbortTransactionAsync();
            throw;
        }
    }
}
