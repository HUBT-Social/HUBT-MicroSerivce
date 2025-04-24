namespace Chat_Data_API.Src.Hubs
{
    public interface IUserConnectionManager
    {
        void AddConnection(string userName, string connectionId);
        void RemoveConnection(string userName);
        string? GetConnectionId(string userName);
        List<string> GetAllOnlineUsers();
    }
}
