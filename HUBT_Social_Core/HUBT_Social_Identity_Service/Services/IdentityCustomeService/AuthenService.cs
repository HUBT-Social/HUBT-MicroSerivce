using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.LoginRequest;
using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace HUBT_Social_Identity_Service.Services.IdentityCustomeService
{
    internal class AuthenService<TUser,TRole>(
        SignInManager<TUser> signInManager,
        RoleManager<TRole> roleManager, 
        UserManager<TUser> userManager) : IAuthenService<TUser,TRole>
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
    {
        private readonly UserManager<TUser> _userManager = userManager;
        private readonly RoleManager<TRole> _roleManager = roleManager;
        private readonly SignInManager<TUser> _signInManager = signInManager;

        public async Task<(SignInResult, TUser?)> LoginAsync(ILoginRequest model)
        {
            var user = await FindUserByIdentifierAsync(model);
            if (user == null || user.UserName == null)
                return (SignInResult.Failed, null);

            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, false, false);
            return result.Succeeded || result.RequiresTwoFactor
                ? (result, user)
                : (result, null);
        }
        public async Task<(IdentityResult Result, TUser? user)> RegisterAsync(RegisterRequest model)
        {
            if (model == null)
                return (IdentityResult.Failed(new IdentityError { Description = "Model không thể null." }), null);

            try
            {
                // Kiểm tra tài khoản đã tồn tại

                if (await CheckUserAccountExit(model))
                    return (IdentityResult.Failed(new IdentityError { Description = "Tài khoản đã được đăng ký." }), null);

                // Tạo người dùng mới
                var user = new TUser
                {
                    UserName = model.UserName,
                    Email = model.Email
                };


                // Tạo tài khoản và kiểm tra kết quả
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    Console.WriteLine(errors);
                    return (result, null);
                }

                // Xác định vai trò mặc định
                const string defaultRole = "USER";

                // Kiểm tra và tạo vai trò nếu chưa có
                if (!await _roleManager.RoleExistsAsync(defaultRole))
                {
                    var roleResult = await _roleManager.CreateAsync(new TRole { Name = defaultRole });
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                        Console.WriteLine(roleErrors);
                        return (IdentityResult.Failed(), null);
                    }
                }

                // Thêm Claims cho người dùng
                var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.UserName),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email)
            };

                // Gán vai trò và Claims cho người dùng
                var roleAssignmentResult = await _userManager.AddToRoleAsync(user, defaultRole);
                if (!roleAssignmentResult.Succeeded)
                {
                    var roleAssignErrors = string.Join("; ", roleAssignmentResult.Errors.Select(e => e.Description));
                    Console.WriteLine(roleAssignErrors);
                    return (IdentityResult.Failed(), null);
                }

                var claimResult = await _userManager.AddClaimsAsync(user, claims);
                if (!claimResult.Succeeded)
                {
                    var claimErrors = string.Join("; ", claimResult.Errors.Select(e => e.Description));
                    Console.WriteLine(claimErrors);
                    return (IdentityResult.Failed(), null);
                }

                return (IdentityResult.Success, user);
            }
            catch (Exception ex)
            {
                // Bắt các lỗi bất ngờ và trả về thông báo lỗi
                Console.WriteLine(ex);
                return (IdentityResult.Failed(new IdentityError { Description = ex.Message }), null);
            }
        }
        private async Task<bool> CheckUserAccountExit(RegisterRequest model)
        {
            return await _userManager.FindByNameAsync(model.UserName) != null ||
                   await _userManager.FindByEmailAsync(model.Email) != null;
        }
        private async Task<TUser?> FindUserByIdentifierAsync(ILoginRequest identifier)
        {
            TUser? user;
            if (new EmailAddressAttribute().IsValid(identifier.Identifier))
            {
                user = await _userManager.FindByEmailAsync(identifier.Identifier);
            }

            else
            {
                user = await _userManager.FindByNameAsync(identifier.Identifier);
            }

            return user;
        }
    }
}
