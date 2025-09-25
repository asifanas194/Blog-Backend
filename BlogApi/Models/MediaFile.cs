using System.ComponentModel.DataAnnotations;

namespace BlogApi.Models
{
    public class MediaFile
    {
        public int Id { get; set; }
        [MaxLength(255)]
        public string FileName { get; set; }
        [MaxLength(1000)]
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
    }


}
