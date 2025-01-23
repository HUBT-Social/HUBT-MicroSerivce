using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace Auth_API.Src.Models
{
    public class DataSignIn
    {
        public SignInResultModel? Result { get; set; }
        public AUserDTO? User { get; set; }
    }
}
