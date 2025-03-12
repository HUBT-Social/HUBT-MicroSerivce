
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Core.Settings;


namespace HUBT_Social_Chat_Resources.Models
{
    public class Participant
    {
        public string UserId { get; set; } = string.Empty;
        public ParticipantRole Role { get; set; }
        public string NickName { get; set; } = string.Empty;
        public string? ProfilePhoto { get; set; }
        public string DefaultAvatarImage { get; set; } = LocalValue.Get(KeyStore.DefaultUserImage);

    }
}
