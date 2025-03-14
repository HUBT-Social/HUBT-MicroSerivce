using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Decode
{

    public static class TokenDecodeExtension
    {
        public static ClaimsPrincipal? DecodeToken(this string accessToken, JwtSetting jwtSettings, out SecurityToken? securityToken, bool refresh = false)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.UTF8.GetBytes(refresh ? jwtSettings.RefreshSecretKey : jwtSettings.SecretKey);

                return tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience
                }, out securityToken);
            }
            catch
            {
                securityToken = null;
                return null;
            }
        }
        public static ClaimsPrincipal? DecodeToken(this string accessToken, JwtSetting jwtSettings,bool refresh = false)
        {
            return DecodeToken(accessToken, jwtSettings, out _,refresh);
        }
        public static string? ExtractBearerToken(this IHeaderDictionary headers)
        {
            var authHeader = headers.Authorization.FirstOrDefault();
            return authHeader?.Replace("Bearer ", "");
        }
        public static TokenInfoDTO? ExtractTokenInfo(this HttpRequest request, JwtSetting jwtSetting)
        {
            // Thử lấy token từ header trước
            string? token = request.Headers.ExtractBearerToken();

            // Nếu không có trong header, lấy từ query string
            if (string.IsNullOrEmpty(token))
            {
                token = request.Query["access_token"].FirstOrDefault();
                Console.WriteLine($"Token extracted from query string: {token}");
            }

            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("No token found in header or query string.");
                return null;
            }

            var claimsPrincipal = token.DecodeToken(jwtSetting);
            if (claimsPrincipal == null)
            {
                Console.WriteLine("Token decoding failed.");
                return null;
            }

            var username = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value;
            var tokenId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            var roles = claimsPrincipal.FindAll(ClaimTypes.Role)
                ?.Select(c => c.Value)
                .Distinct()
                .ToArray() ?? [];

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(tokenId))
            {
                Console.WriteLine("Token missing required claims.");
                return null;
            }

            return new TokenInfoDTO
            {
                Username = username,
                UserId = userId,
                Email = email,
                TokenId = tokenId,
                Roles = roles
            };
        }
    }
}

