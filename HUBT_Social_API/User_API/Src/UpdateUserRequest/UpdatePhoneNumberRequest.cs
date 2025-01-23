using System.ComponentModel.DataAnnotations;

namespace User_API.Src.UpdateUserRequest;

public class UpdatePhoneNumberRequest
{

    [Phone] public string PhoneNumber { get; set; } = string.Empty;
}