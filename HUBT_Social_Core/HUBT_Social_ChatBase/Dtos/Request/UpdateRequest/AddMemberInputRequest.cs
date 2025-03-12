namespace ChatBase.Dtos.Request.UpdateRequest
{
    public class AddMemberInputRequest
    {
        public string GroupId { get; set; } = String.Empty;
        public string AddedId { get; set; }
    }
}
