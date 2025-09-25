namespace BlogApi.DTOs
{
    public class UpdatePostDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string? ImagePath { get; set; }
        public string? VideoPath { get; set; }
    }

}
