using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using System.Text.RegularExpressions;

namespace HUBT_Social_Core.Models.Requests.LoginRequest
{
    public class SignInResponse
    {
        private string _maskEmail = string.Empty;

        public TokenResponseDTO? UserToken { get; set; } = null;

        public string MaskEmail
        {
            get => _maskEmail;
            set => _maskEmail = EncodeEmail(value);
        }

        public string Message { get; set; } = string.Empty;
        public bool RequiresTwoFactor { get; set; }

        private static string EncodeEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return string.Empty;
                string[] emailParts = email.Split('@');

                if (emailParts.Length == 2)
                {
                    string username = emailParts[0];
                    string domain = emailParts[1];

                    string maskedUsername = string.Concat(username.AsSpan(0, 3), new string('*', Math.Max(0, username.Length)));

                    var domainParts = domain.Split('.');
                    string maskedDomain = new('*', domainParts[0].Length);

                    return $"{maskedUsername}@{maskedDomain}.{domainParts[1]}";
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
            return string.Empty;


        }
    }
}
