namespace Chat_API.Src.Constants
{
    public static class ChatApiEndpoints
    {
        public const string ChatService_CreateGroup = "api/chat/create-group";
        public const string ChatService_DeleteGroup = "api/chat/delete-group";
        public const string ChatService_SearchGroups = "api/chat/search-groups";
        public const string ChatService_GetAllRooms = "api/chat/get-all-rooms";
        public const string ChatService_GetRoomsOfUser = "api/chat/get-rooms-of-user";

        public const string RoomService_UpdateGroupName = "api/room/update-group-name";
        public const string RoomService_UpdateNickName = "api/room/update-nickname";
        public const string RoomService_UpdateParticipantRole = "api/room/update-participant-role";
        public const string RoomService_JoinRoom = "api/room/join-room";
        public const string RoomService_KickMember = "api/room/kick-member";
        public const string RoomService_LeaveRoom = "api/room/leave-room";
        public const string RoomService_GetMessageHistory = "api/room/message-history";
        public const string RoomService_GetRoomUser = "api/room/get-members";

        public const string UserService_GetUser = "api/user";
        public const string UserService_GetAllUser = "api/user/userAll";
    }
}
