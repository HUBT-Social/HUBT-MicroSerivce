
namespace HUBT_Social_Core.Models.DTOs.EmailDTO
{
    public class EmailRequest
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Device { get; set; } = string.Empty;
        public string? Location { get; set; } = string.Empty;
        public string DateTime { get; set; } = string.Empty;
    }
}
