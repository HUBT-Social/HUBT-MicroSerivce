using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace User_API.Src.UpdateUserRequest
{
    public class StoreFCMRequest
    {
        [Required(ErrorMessage = "FcmToken không được để trống.")]
        public string FcmToken { get; set; } = string.Empty;
    }
}
