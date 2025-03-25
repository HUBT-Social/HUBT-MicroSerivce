using System.ComponentModel.DataAnnotations;

namespace Auth_API.Src.Models.Requests;

public class SearchUserByUserNameOrPasswordRequest
{
    [Required] public string UserNameOrEmail { get; set; } = string.Empty;
}