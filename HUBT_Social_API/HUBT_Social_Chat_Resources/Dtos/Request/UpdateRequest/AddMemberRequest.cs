using HUBT_Social_Chat_Resources.Models;

namespace HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest
{
    public class AddMemberRequest
    {
        public string GroupId { get; set; } = String.Empty;
        public string Added { get; set; } = String.Empty;
    }
    public class AddMemberRequestData
    {
        public string GroupId { get; set; } = String.Empty;
        public Participant? Participant { get; set; }
    }
}
