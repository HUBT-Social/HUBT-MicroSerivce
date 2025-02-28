using System.Threading.Tasks;
using Amazon.Runtime;
using FirebaseAdmin.Auth;
using HUBT_Social_API.Core.Settings;
using HUBT_Social_API.Features.Auth.Models;
using HUBTSOCIAL.Src.Features.Chat.Collections;
using HUBTSOCIAL.Src.Features.Chat.Helpers;
using Microsoft.AspNetCore.Identity;

namespace HUBTSOCIAL.Src.Features.Chat.Models;


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
        this.Role = role??ParticipantRole.Member;
    }

    public static async Task<Participant> CreateAsync(UserManager<AUser> userManager, string userId, ParticipantRole? role)
    {
        var participant = new Participant(userId, role);
        participant.NickName = await UserHelper.GetFullNameById(userManager, userId);
        participant.ProfilePhoto = await UserHelper.GetAvatarUserById(userManager,userId);
        return participant;
    }
}
