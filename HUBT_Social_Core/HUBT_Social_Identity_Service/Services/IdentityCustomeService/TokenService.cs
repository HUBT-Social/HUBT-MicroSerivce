using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_Identity_Service.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Identity_Service.Services.IdentityCustomeService
{
    internal class TokenService<TUser,TToken>(
        UserManager<TUser> userManager,
        IOptions<JwtSetting> jwtSettings,
        IMongoCollection<TToken> userToken
        ) : ITokenService<TUser,TToken> 
        where TUser : MongoIdentityUser<Guid>, new()
        where TToken : IdentityToken, new()
    {
        private readonly JwtSetting _jwtSetting = jwtSettings.Value;
        private readonly UserManager<TUser> _userManager = userManager;
        private readonly IMongoCollection<TToken> _tokenManager = userToken;
        // Tạo JWT token và handle Refresh Token
        public async Task<TokenResponseDTO?> GenerateTokenAsync(TUser user)
        {
            List<Claim> claims = [.. await _userManager.GetClaimsAsync(user)];

            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role));
            claims.AddRange(roleClaims);

            // Tạo JWT token
            var token = GenerateAccessToken(claims);
            var refreshToken = GenerateRefreshToken(claims);

            await HandleRefreshTokenAsync(user, token, refreshToken);

            return new TokenResponseDTO
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtSetting.TokenExpirationInMinutes,
                TokenType = "bearer"
            };
        }

        // Tạo JWT Token
        private string GenerateToken(IEnumerable<Claim> claims, string secretKey, Func<DateTime> expiration)
        {
            // Kiểm tra giá trị của SecretKey
            if (string.IsNullOrEmpty(secretKey)) throw new ArgumentException("SecretKey must not be null or empty.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiration(),
                SigningCredentials = credentials,
                Issuer = _jwtSetting.Issuer,
                Audience = _jwtSetting.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            return GenerateToken(
                claims,
                _jwtSetting.SecretKey,
                () => DateTime.UtcNow.AddMinutes(_jwtSetting.TokenExpirationInMinutes)
            );
        }

        private string GenerateRefreshToken(IEnumerable<Claim> claims)
        {
            return GenerateToken(
                claims,
                _jwtSetting.RefreshSecretKey,
                () => DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationInDays)
            );
        }

        public async Task<TUser?> GetAUserDTO(string accessToken)
        {
            ClaimsPrincipal? principal = accessToken.DecodeToken(_jwtSetting);
            if (principal != null)
            {
                var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (!string.IsNullOrEmpty(userIdClaim))
                {
                    TUser? user = await _userManager.FindByIdAsync(userIdClaim);
                    if (user != null) return user;

                }

            }
            return null;

        }
        private async Task HandleRefreshTokenAsync(TUser user, string accessToken, string refreshToken)
        {
            var existingRefreshToken = await _tokenManager.Find(t => t.UserId == user.Id.ToString()).FirstOrDefaultAsync();

            if (existingRefreshToken == null)
            {
                await _tokenManager.InsertOneAsync(new TToken
                {
                    AccessToken = accessToken,
                    RefreshTo = refreshToken,
                    UserId = user.Id.ToString(),
                    ExpireTime = DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationInDays)
                });
            }
            else
            {
                var update = Builders<TToken>.Update.Set(t => t.AccessToken, accessToken)
                    .Set(t => t.RefreshTo, refreshToken)
                    .Set(t => t.ExpireTime, DateTime.UtcNow.AddDays(_jwtSetting.RefreshTokenExpirationInDays));
                await _tokenManager.UpdateOneAsync(t => t.UserId == existingRefreshToken.UserId, update);
            }
        }

        public async Task<TokenResponseDTO?> ValidateTokens(string accessToken, string refreshToken)
        {
            ClaimsPrincipal? accessPrincipal = accessToken.DecodeToken(_jwtSetting);
            string? accessUserId = accessPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            ClaimsPrincipal? refreshPrincipal = refreshToken.DecodeToken(_jwtSetting,refresh:true);
            string? refreshUserId = refreshPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (refreshUserId != null && accessUserId == refreshUserId)
            {
                TUser? user = await _userManager.FindByIdAsync(refreshUserId);
                if (user != null)
                {
                    var existingRefreshToken = await _tokenManager.Find(t => t.UserId == user.Id.ToString() && t.RefreshTo == refreshToken).FirstOrDefaultAsync();
                    if (existingRefreshToken != null)
                    {
                        return await GenerateTokenAsync(user);
                    }
                    Console.WriteLine($"Refresh token not user's refresh token: {user.UserName}");
                }
                Console.WriteLine($"User not found with id : {refreshUserId}");

            }
            Console.WriteLine($"accessUserID and RefreshUserId doesnot match. access : {accessUserId} Refresh :{refreshUserId}");
            return null;
        }


        public async Task<bool> DeleteTokenAsync(TUser user)
        {
            FilterDefinition<TToken> filter = Builders<TToken>.Filter.Eq("_id", user.Id.ToString());
            DeleteResult result = await _tokenManager.DeleteOneAsync(filter);

            if (result.DeletedCount > 0)
            {
                Console.WriteLine("Xóa thành công.");
                return true;
            }
            return false;
        }
    }
}
