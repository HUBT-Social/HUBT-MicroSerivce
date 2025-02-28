namespace HUBT_Social_API.Features.Chat.Services.Interfaces;
public interface IUserConnectionManager
{
    void AddConnection(string userName, string connectionId);
    void RemoveConnection(string userName);
    string? GetConnectionId(string userName);
}