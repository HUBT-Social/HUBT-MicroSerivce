using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Helper
{
    public static class TokenHelper
    {
        private static JwtSetting _jwtSetting;

        public static void Configure(IOptions<JwtSetting> jwtSetting)
        {
            _jwtSetting = jwtSetting.Value ?? throw new ArgumentNullException(nameof(jwtSetting));
        }

        public static TokenInfoDTO? GetUserInfoFromRequest(this HttpRequest request)
        {
            if (_jwtSetting == null)
                throw new InvalidOperationException("TokenHelper must be configured. Call Configure first.");

            try
            {
                return request.ExtractTokenInfo(_jwtSetting);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting token info: {ex.Message}");
                return null;
            }
        }

    }
}