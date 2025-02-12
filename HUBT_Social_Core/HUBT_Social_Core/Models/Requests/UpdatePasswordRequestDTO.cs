using System.ComponentModel.DataAnnotations;

namespace HUBT_Social_Core.Models.Requests;

public class UpdatePasswordRequestDTO
{
    [Required, DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}