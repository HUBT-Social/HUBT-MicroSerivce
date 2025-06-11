using System.ComponentModel.DataAnnotations;

namespace User_API.Src.Models
{
    public class FileUploadModel
    {
        [Required]
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = "Môn học giúp bạn có thể cải thiện kỹ năng";
        public string ImageUrl { get; set; } = "https://cdn.pixabay.com/photo/2016/10/25/12/28/chemistry-1762804_1280.png";
        [Required]
        public string Major { get; set; } = string.Empty;
        public int Credits { get; set; } = 2;
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
