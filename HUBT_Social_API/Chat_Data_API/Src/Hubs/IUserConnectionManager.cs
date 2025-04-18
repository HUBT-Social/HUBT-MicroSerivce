﻿namespace Chat_Data_API.Src.Hubs
{
    public interface IUserConnectionManager
    {
        void AddConnection(string userId, string connectionId);
        void RemoveConnection(string userId);
        string? GetConnectionId(string userId);
        List<string> GetAllOnlineUsers();
    }
}
