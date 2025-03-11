namespace HUBT_Social_Chat_Resources.Dtos.Collections
{
    public class Reaction
    {
        public List<string> Reactions { get; set; } = new List<string>();
        public List<string> reactedUserIds { get; set; } = new List<string>();

    }
}
