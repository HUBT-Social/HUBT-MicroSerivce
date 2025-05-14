using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Firebase;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using MongoDB.Driver.Core.Operations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Identity_Service.Services.IdentityCustomeService
{
    internal class UserService<TUser, TRole>(UserManager<TUser> userManager, RoleManager<TRole> roleManager) : IUserService<TUser, TRole>
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
    {
        private readonly UserManager<TUser> _userManager = userManager;
        private readonly RoleManager<TRole> _roleManager = roleManager;

        private readonly Dictionary<string, int> _roleHierarchy = new()
        {
            { "ADMIN", 4 },
            { "HEAD", 3 },
            { "TEACHER", 2 },
            { "STUDENT", 1 },
            { "USER", 0 }
        };
        public async Task<TUser?> FindUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<TUser?> FindUserByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            return await _userManager.FindByIdAsync(id);
        }
        public async Task<TUser?> FindUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            return await _userManager.FindByNameAsync(userName);
        }
        public async Task<bool> EnableTwoFactor(string userName)
        {
            TUser? user = await GetUserByNameAsync(userName);
            if (user != null)
            {
                IdentityResult result = await UpdateUserPropertyAsync(user, u => u.TwoFactorEnabled = true);
                return user != null && result.Succeeded;
            }
            return false;
        }

        public async Task<bool> DisableTwoFactor(string userName)
        {
            TUser? user = await GetUserByNameAsync(userName);
            if (user != null) 
            {
                IdentityResult result = await UpdateUserPropertyAsync(user, u => u.TwoFactorEnabled = false);
                return user != null && result.Succeeded;
            } 
            return false;
            
        }
        public async Task<bool> UpdatePasswordAsync(string userName, UpdatePasswordRequestDTO request)
        {
            try
            {
                var user = await GetUserByNameAsync(userName);
                if (user == null) return false;

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, request.NewPassword);

                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> DeleteUserAsync(TUser user)
        {
            IdentityResult deleted = await _userManager.DeleteAsync(user);
            return deleted.Succeeded;
        }
        public async Task<bool> UpdateUserAsync(TUser user)
        {
            IdentityResult result = await _userManager.UpdateAsync(user);
            return result.Succeeded && user != null;
        }

        private async Task<IdentityResult> UpdateUserPropertyAsync(TUser user, Action<TUser> updateAction)
        {
            try
            {
                updateAction(user);
                return await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(
                    new IdentityError 
                    { 
                        Description = ex.Message 
                    });

            }
        }
        private async Task<TUser?> GetUserByNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<bool> PromoteUserAccountAsync(string currentUserName, string targetUserName, string roleName)
        {
            try
            {
                if (!IsValidRole(roleName) || string.IsNullOrEmpty(targetUserName)) return false;

                var currentUser = await GetUserByNameAsync(currentUserName);
                var targetUser = await GetUserByNameAsync(targetUserName);
                if (currentUser == null || targetUser == null) return false;

                var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
                var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

                var currentUserHighestRole = GetHighestRole(currentUserRoles);
                var targetUserHighestRole = GetHighestRole(targetUserRoles);
                if (currentUserHighestRole != null && targetUserHighestRole != null)
                {
                    if (_roleHierarchy[currentUserHighestRole] > _roleHierarchy[targetUserHighestRole])
                    {
                        await EnsureRoleExistsAsync(roleName);
                        var result = await _userManager.AddToRoleAsync(targetUser, roleName);
                        return result.Succeeded;
                    }
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool IsValidRole(string roleName) => _roleHierarchy.ContainsKey(roleName);

        private async Task EnsureRoleExistsAsync(string roleName)
        {
            if (!await _roleManager.RoleExistsAsync(roleName))
            {
                var role = new TRole { Name = roleName };
                await _roleManager.CreateAsync(role);
            }
        }
        private string? GetHighestRole(IList<string> roles)
        {
            return roles.OrderByDescending(role => _roleHierarchy.GetValueOrDefault(role, 0)).FirstOrDefault();
        }

        public List<TUser>? GetAll()
        {
            var userlist  = _userManager.Users.ToList<TUser>();
            return userlist ?? null;
        }
        public async Task<(List<TUser>,bool,string?)> GetUserByRole(string RoleName,int page = 0)
        {
            bool hasMore = true;
            int pageSize = 100;
            var role = _roleManager.Roles
                .Where(r => r.Name == RoleName)
                .FirstOrDefault();

            if (role == null) return (new List<TUser>(),hasMore,"Role khong hop le.");
            int quantityUser = _userManager.Users.Count();
            if ((page+1)*pageSize - quantityUser >= pageSize) 
            {
                return (new List<TUser>(), !hasMore,null);
            }
            var users = _userManager.Users
                .Skip(page * pageSize)
                .Take(pageSize)
                .Where(u => u.Roles.Contains(role.Id))
                .ToList();


            return (users, hasMore, null);
        }

        public async Task<bool> CheckRole(string userName, string roleName)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(roleName))
            {
                return false;
            }

            var user = await GetUserByNameAsync(userName);
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);

            return roles.Contains(roleName);
        }


    }
}