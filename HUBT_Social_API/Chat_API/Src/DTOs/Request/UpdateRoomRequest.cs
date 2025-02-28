using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.DTOs;
public class UpdateNickNameRequest
{
    public string GroupId { get; set; } = String.Empty;
    public string UserId { get; set; } = String.Empty;
    public string NewNickName { get; set; } = String.Empty;
}
public class UpdateGroupNameRequest
{
    public string Id { get; set; } = String.Empty;
    public string NewName { get; set; } = String.Empty;
}
public class UpdateAvatarRequest
{
    public string Id { get; set; } = String.Empty;
    public FormFile file { get; set; } 
}

    