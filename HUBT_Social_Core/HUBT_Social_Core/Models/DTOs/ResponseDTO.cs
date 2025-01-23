using System.Net;

namespace HUBT_Social_Core.Models.DTOs
{
    public class ResponseDTO
    {
        public object? Data { get; set; }
        public string Message { get; set; } = string.Empty;

        public HttpStatusCode StatusCode { get; set; }
    }
}
