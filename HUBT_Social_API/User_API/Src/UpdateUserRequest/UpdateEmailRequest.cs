using System.ComponentModel.DataAnnotations;

namespace User_API.Src.UpdateUserRequest;

public class UpdateEmailRequest
{

    [EmailAddress] public string Email { get; set; } = string.Empty;
}