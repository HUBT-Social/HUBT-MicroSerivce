namespace Chat_Data_API.Src.Hubs
{
    public class UserConnectionManager : IUserConnectionManager
    {
        private readonly Dictionary<string, string> _userConnections = new Dictionary<string, string>();

        public void AddConnection(string userName, string connectionId)
        {
            lock (_userConnections)
            {
                _userConnections[userName] = connectionId;
            }
        }

        public void RemoveConnection(string userName)
        {
            lock (_userConnections)
            {
                if (_userConnections.ContainsKey(userName))
                {
                    _userConnections.Remove(userName);
                }
            }
        }

        public string? GetConnectionId(string userName)
        {
            lock (_userConnections)
            {
                _userConnections.TryGetValue(userName, out var connectionId);
                return connectionId;
            }
        }

        public List<string> GetAllOnlineUsers()
        {
            return _userConnections.Keys.ToList();
        }
    }
}
