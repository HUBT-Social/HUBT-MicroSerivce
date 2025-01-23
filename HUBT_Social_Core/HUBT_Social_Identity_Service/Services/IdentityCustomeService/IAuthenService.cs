using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using HUBT_Social_Core.Models.Requests;


namespace HUBT_Social_Identity_Service.Services.IdentityCustomeService
{
    public interface IAuthenService<TUser,TRole>
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
    {
        Task<(SignInResult Result, TUser? User)> LoginAsync(ILoginRequest model);
        Task<(IdentityResult Result, TUser? user)> RegisterAsync(RegisterRequest model);
    }
}
