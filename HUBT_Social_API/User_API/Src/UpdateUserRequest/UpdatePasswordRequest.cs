using System.ComponentModel.DataAnnotations;

namespace User_API.Src.UpdateUserRequest;

public class UpdatePasswordRequest
{
    [Required,DataType(DataType.Password)]
    public string NewPassword { get; set; } = string.Empty;

    [DataType(DataType.Password), Compare(nameof(NewPassword))]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}