namespace HUBT_Social_Chat_Resources.Dtos.Response
{
    public class GetMemberGroup
    {
        public string caller { get; set; } = string.Empty;
        public List<ChatUserResponse> response { get; set; }
    }
}