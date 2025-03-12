namespace ChatBase.Dtos.Request.UpdateRequest
{
    public class UpdateNickNameRequest
    {
        public string GroupId { get; set; } = String.Empty;
        public string UserId { get; set; } = String.Empty;
        public string NewNickName { get; set; } = String.Empty;
    }
}
