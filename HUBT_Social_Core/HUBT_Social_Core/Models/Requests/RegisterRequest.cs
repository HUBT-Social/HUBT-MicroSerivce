using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Core.Models.Requests
{
    public class RegisterRequest
    {
        [Required] public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}
