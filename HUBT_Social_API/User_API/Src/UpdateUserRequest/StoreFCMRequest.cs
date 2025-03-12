using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HUBT_Social_API.Src.Features.Auth.Dtos.Request
{
    public class StoreFCMRequest
    {
        [Required(ErrorMessage = "FcmToken không được để trống.")]
        public string FcmToken { get; set; } = string.Empty;
    }
}
