using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.UpdateUserRequest;

public class AddInfoUserRequest
{

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public Gender Gender { get; set; } = 0;

    public DateTime DateOfBirth { get; set; } // Ngày sinh
}