namespace Chat_API.Src.Model
{
    public class UserConnection
    {
        public string UserName { get; set; } = string.Empty;
        public string ChatRoom { get; set; } = string.Empty;
        public bool IsConnected { get; set; }
    }
}
