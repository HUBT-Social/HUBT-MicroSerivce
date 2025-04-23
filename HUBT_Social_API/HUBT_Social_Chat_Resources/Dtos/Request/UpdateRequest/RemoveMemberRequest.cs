namespace HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest
{
    public class RemoveMemberRequest
    {
        public string GroupId { get; set; } = String.Empty;
        public string Kicked { get; set; } = String.Empty;
    }
}
