using Microsoft.AspNetCore.Mvc;

namespace User_API.Src.UpdateUserRequest;

public class UpdateAvatarUrlRequest
{
    [FromForm]
    public string AvatarUrl { get; set; } = string.Empty;

}