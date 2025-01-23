using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Identity_Service.Models;
using HUBT_Social_Identity_Service.Services.IdentityCustomeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Identity_Service.Services
{
    internal class HubtIdentityService<TUser, TRole>(
        IUserService<TUser, TRole> userService,
        IAuthenService<TUser, TRole> authService) : IHubtIdentityService<TUser, TRole>
    where TUser : MongoIdentityUser<Guid>, new()
    where TRole : MongoIdentityRole<Guid>, new()
    {
        public IUserService<TUser, TRole> UserService { get; } = userService;
        public IAuthenService<TUser, TRole> AuthService { get; } = authService;
    }

    internal class HubtIdentityService<TUser, TRole, TToken>(
        IUserService<TUser, TRole> userService,
        IAuthenService<TUser, TRole> authService,
        ITokenService<TUser, TToken> tokenService) :
        IHubtIdentityService<TUser, TRole, TToken>
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
        where TToken : IdentityToken, new()
    {
        public IUserService<TUser, TRole> UserService { get; } = userService;
        public IAuthenService<TUser, TRole> AuthService { get; } = authService;
        public ITokenService<TUser, TToken> TokenService { get; } = tokenService;
    }

}
