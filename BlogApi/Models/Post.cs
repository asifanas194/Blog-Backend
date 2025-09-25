using BlogApi.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;

    public string? ImagePath { get; set; } 
    public string? VideoPath { get; set; } 

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Foreign Key
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
