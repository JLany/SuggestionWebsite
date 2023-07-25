namespace SuggestionAppLibrary.Models;

public class PostModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }  
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public CategoryModel Category { get; set; }
    public StatusModel Status { get; set; }
    public PostAuthorViewModel Author { get; set; }
    public HashSet<string> UserVotes { get; set; } = new();
    public string AdminNotes { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
}
