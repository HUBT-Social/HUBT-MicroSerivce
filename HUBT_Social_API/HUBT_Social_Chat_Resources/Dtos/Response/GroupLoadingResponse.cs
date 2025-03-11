namespace HUBT_Social_Chat_Resources.Dtos.Response
{
    public class GroupLoadingResponse : GroupBaseResponse
    {
        public string? LastMessage { get; set; } = string.Empty;
        public string? LastInteractionTime { get; set; } = String.Empty;
    }
}
