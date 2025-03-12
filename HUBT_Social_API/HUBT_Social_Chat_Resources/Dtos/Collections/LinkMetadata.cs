namespace HUBT_Social_Chat_Resources.Dtos.Collections
{
    public class LinkMetadataModel
    {
        public string Url { get; set; } = string.Empty; // URL của liên kết
        public string Title { get; set; } = "No Title"; // Tiêu đề của liên kết
        public string Description { get; set; } = string.Empty; // Mô tả của liên kết
        public string Thumbnail { get; set; } = string.Empty; // URL của ảnh thumbnail
    }
}
