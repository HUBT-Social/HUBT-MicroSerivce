using ChatBase.Dtos.Collections.Enum;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Identity;

namespace ChatBase.Models
{
    public class Participant
    {
        public string UserId { get; set; } = string.Empty;
        public ParticipantRole Role { get; set; }
        public string NickName { get; set; } = string.Empty;
        public string? ProfilePhoto { get; set; }
        public string DefaultAvatarImage { get; set; } = LocalValue.Get(KeyStore.DefaultUserImage);

        // Constructor private để bắt buộc dùng phương thức CreateAsync
        private Participant(string userId, ParticipantRole? role)
        {
            this.UserId = userId;
            this.Role = role ?? ParticipantRole.Member;
        }

        //public static async Task<Participant> CreateAsync(UserManager<AUser> userManager, string userId, ParticipantRole? role)
        //{
        //    var participant = new Participant(userId, role);
        //    participant.NickName = await UserHelper.GetFullNameById(userManager, userId);
        //    participant.ProfilePhoto = await UserHelper.GetAvatarUserById(userManager, userId);
        //    return participant;
        //}
    }
}
