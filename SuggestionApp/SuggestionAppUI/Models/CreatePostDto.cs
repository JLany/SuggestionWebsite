using System.ComponentModel.DataAnnotations;

namespace SuggestionAppUI.Models;

public class CreatePostDto
{ 
	[Required]
	[MaxLength(75)]
	public string Title { get; set; }

	[Required]
	[MinLength(1)]
	[Display (Name = "Category")]
	public string CategoryId { get; set; }

	[MaxLength(500)]
	public string Description { get; set; }
}
