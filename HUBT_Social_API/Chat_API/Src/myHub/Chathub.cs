using Chat_API.Src.Model;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Chat_API.Src.myHub
{
    public class Chathub : Hub
    {
        private static ConcurrentDictionary<string, UserConnection> userConnections = new ConcurrentDictionary<string, UserConnection>();

        public override async Task OnConnectedAsync()
        {
            userConnections.TryRemove(Context.ConnectionId, out var connection);
            connection.IsConnected = true;
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has connected");
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {

            userConnections.TryRemove(Context.ConnectionId, out var connection);
            connection.IsConnected = false;
            if (connection != null)
            {
                await Clients.Group(connection.ChatRoom).SendAsync("groupMessage", $"{connection.UserName} has left the chat");
            }

            await base.OnDisconnectedAsync(exception);
        }


        public async Task SendMessageToGroup(string message)
        {
    
            await Clients.All.SendAsync("CharRecive", $"{Context.ConnectionId} says: {message}");
            
        }

        public async Task JoinSpecificChatGroup(UserConnection connection)
        {

            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);


            userConnections[Context.ConnectionId] = connection;

            await Clients.Group(connection.ChatRoom).SendAsync("JoinSpecificChatGroup", $"{connection.UserName} has joined {connection.ChatRoom}");
        }

    }
}
