namespace ChatBase.Dtos.Request.UpdateRequest
{
    public class RemoveMemberRequest
    {
        public string GroupId { get; set; } = String.Empty;
        public string KickedId { get; set; } = String.Empty;
    }
}
