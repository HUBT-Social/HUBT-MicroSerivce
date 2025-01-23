using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Core.Models.Requests.LoginRequest;

public class LoginByEmailRequest : ILoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    public string Identifier => Email;
}