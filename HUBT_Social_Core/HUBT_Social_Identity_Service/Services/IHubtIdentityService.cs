using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Identity_Service.Models;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Identity_Service.Services
{
    public interface IHubtIdentityService<TUser, TRole>
    where TUser : MongoIdentityUser<Guid>, new()
    where TRole : MongoIdentityRole<Guid>, new()
    {
        IUserService<TUser, TRole> UserService { get; }
        IAuthenService<TUser, TRole> AuthService { get; }
    }

    // Interface mở rộng khi có TokenService
    public interface IHubtIdentityService<TUser, TRole, TToken> : IHubtIdentityService<TUser, TRole>
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
        where TToken : IdentityToken, new()
    {
        ITokenService<TUser, TToken> TokenService { get; }
    }
}
