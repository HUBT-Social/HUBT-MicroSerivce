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

        public const string Indentity_GetUser = "api/identity/user";
        public const string Indentity_GetAllUser = "api/identity/user/userAll";
        public const string Indentity_GetUserByUserName = "api/identity/user/get";
        public const string Indentity_GetUserByUserNames = "api/identity/users-in-list-userName";

        public const string TempService_GetCourse = "api/tempCourse";

        public const string OutService_GetUsersByClassName = "api/hubt/{{className}}";
    }
}
