namespace ChatBase.Dtos.Request.UpdateRequest
{
    public class AddMemberRequest
    {
        public string GroupId { get; set; } = String.Empty;
        public Participant Added { get; set; }
    }
}
