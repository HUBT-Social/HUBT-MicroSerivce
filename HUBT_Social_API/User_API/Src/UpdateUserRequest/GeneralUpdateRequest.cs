using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.UpdateUserRequest;

public class GeneralUpdateRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    public Gender Gender { get; set; }

    public DateTime DateOfBirth { get; set; }
}