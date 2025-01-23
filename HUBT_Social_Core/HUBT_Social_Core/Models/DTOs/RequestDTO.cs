using HUBT_Social_Core.Settings.@enum;

namespace HUBT_Social_Core.Models.DTOs
{
    public class RequestDTO
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string? AccessToken { get; set; }
    }
}
