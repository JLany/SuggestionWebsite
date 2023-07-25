namespace SuggestionAppLibrary.Models;

public class PostAuthorViewModel
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string DisplayName { get; set; }

    public PostAuthorViewModel() { }

    public PostAuthorViewModel(UserModel user)
    {
        Id = user.Id;
        DisplayName = user.DisplayName;
    }
}
