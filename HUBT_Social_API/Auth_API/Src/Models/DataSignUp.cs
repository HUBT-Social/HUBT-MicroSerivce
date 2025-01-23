using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace Auth_API.Src.Models
{
    public class DataSignUp
    {
        public SignUpResultModel? Result { get; set; }
        public AUserDTO? User { get; set; }
    }
}
