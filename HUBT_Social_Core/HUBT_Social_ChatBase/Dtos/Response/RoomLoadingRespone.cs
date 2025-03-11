namespace ChatBase.Dtos.Response
{
    public class RoomLoadingResponse : RoomBaseResponse
    {
        public string LastMessage { get; set; } = string.Empty;
        public string LastInteractionTime { get; set; } = String.Empty;
    }
}
