using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Firebase;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Identity_Service.Services.IdentityCustomeService
{
    public interface IUserService<TUser, TRole>
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
    {

        List<TUser>? GetAll();
        Task<(List<TUser>, bool, string?)> GetUserByRole(string RoleName, int page = 0);
        
        Task<TUser?> FindUserByUserNameAsync(string userName);
        Task<TUser?> FindUserByIdAsync(string id);
        Task<TUser?> FindUserByEmailAsync(string email);


        Task<bool> CheckRole(string userName, string roleName);
        Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName);


        Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequestDTO request);

        Task<bool> UpdateUserAsync(TUser user);
        Task<bool> DeleteUserAsync(TUser user);
        Task<bool> EnableTwoFactor(string userName);

        Task<bool> DisableTwoFactor(string userName);


    }
}
