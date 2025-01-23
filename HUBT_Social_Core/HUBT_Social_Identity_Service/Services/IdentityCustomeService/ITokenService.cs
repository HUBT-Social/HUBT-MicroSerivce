
using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Identity_Service.Models;

namespace HUBT_Social_Identity_Service.Services.IdentityCustomeService;

public interface ITokenService<TUser,TToken>
    where TUser : MongoIdentityUser<Guid>, new()
    where TToken : IdentityToken, new()
{
    Task<TokenResponseDTO?> GenerateTokenAsync(TUser user);
    Task<TUser?> GetAUserDTO(string accessToken);
    
    Task<TokenResponseDTO?> ValidateTokens(string accessToken, string refreshToken);

    Task<bool> DeleteTokenAsync(TUser user);
}