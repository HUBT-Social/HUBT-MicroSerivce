using AspNetCore.Identity.MongoDbCore.Models;
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
        Task<TUser?> FindUserByUserNameAsync(string userName);
        Task<TUser?> FindUserByIdAsync(string id);
        Task<TUser?> FindUserByEmailAsync(string email);
        Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName);

        //Phương thức cập nhật từng trường riêng lẻ
        //Task<bool> UpdateAvatarUrlAsync(string userName, UpdateAvatarUrlRequest request);
        //Task<bool> UpdateEmailAsync(string userName, UpdateEmailRequest request);
        //Task<bool> VerifyCurrentPasswordAsync(string userName, CheckPasswordRequest request);
        //Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequest request);
        //Task<bool> UpdateNameAsync(string userName, UpdateNameRequest request);
        //Task<bool> UpdatePhoneNumberAsync(string userName, UpdatePhoneNumberRequest request);
        //Task<bool> UpdateGenderAsync(string userName, UpdateGenderRequest request);
        //Task<bool> UpdateDateOfBirthAsync(string userName, UpdateDateOfBornRequest request);
        //Task<bool> AddInfoUser(string userName, AddInfoUserRequest request);
        Task<bool> UpdateUserAsync(TUser user);
        Task<bool> DeleteUserAsync(TUser user);
        Task<bool> EnableTwoFactor(string userName);

        Task<bool> DisableTwoFactor(string userName);

        //// Phương thức cập nhật tổng quát
        //Task<bool> GeneralUpdateAsync(string userName, GeneralUpdateRequest request);
    }
}
