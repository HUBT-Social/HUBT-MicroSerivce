using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.DTOs;


public class MessageInputRequest {
    public string GroupId { get; set; } = string.Empty;
    public string Content { get; set; } =string.Empty;
    public ReplyMessage? ReplyToMessage { get; set; } 
}
public class MediaInputRequest
{
    public string GroupId { get; set; } = string.Empty;
    public List<IFormFile> Medias { get; set; }
    public ReplyMessage? ReplyToMessage { get; set; } 
}
public class SendChatRequest
{
    public string RequestId { get; set; } = string.Empty;
    public string GroupId { get; set; } = string.Empty;
    public string? Content { get; set; } =null;
    public List<IFormFile>? Medias { get; set; } = null;
    public List<IFormFile>? Files { get; set; } = null;
    public string? ReplyToMessageId { get; set; }
}

public class ChatRequest : SendChatRequest
{   
    public string UserId { get; set; } = string.Empty;
    
}


public class MessageRequest : MessageInputRequest
{   
    public string UserId { get; set; } = string.Empty;
    
}
public class MediaRequest : MediaInputRequest
{
    public string? UserId { get; set; } = string.Empty;
}


public class DeleteGroupRequest
    {
        public string GroupId { get; set; }
    }

public class AddMemberInputRequest
{
    public string GroupId { get; set; } = String.Empty;
    public string AddedId { get; set; }
}
public class AddMemberRequest
{
    public string GroupId { get; set; } = String.Empty;
    public Participant Added { get; set; }
}

public class RemoveMemberRequest
{
    public string GroupId { get; set; } = String.Empty;
    public string KickedId { get; set; } = String.Empty;
}

public class LeaveRoomRequest
{
    public string GroupId { get; set; } = String.Empty;
}