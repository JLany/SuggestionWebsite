namespace SuggestionAppLibrary.Models;

public class UserPostViewModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string Title { get; set; }

    public UserPostViewModel() { }

    public UserPostViewModel(PostModel post)
    {
        Id = post.Id;
        Title = post.Title; 
    }
}
