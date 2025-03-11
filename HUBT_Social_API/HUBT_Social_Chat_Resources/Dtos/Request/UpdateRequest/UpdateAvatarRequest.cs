using Microsoft.AspNetCore.Http;

namespace HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest
{
    public class UpdateAvatarRequest
    {
        public string Id { get; set; } = String.Empty;
        public FormFile file { get; set; }
    }
}
