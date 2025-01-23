using AspNetCore.Identity.MongoDbCore.Models;
using AutoMapper;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;

namespace HUBT_Social_Identity_Service.Configurations
{
    public class IdentityMapper<TUser,TRole> : Profile
        where TUser : MongoIdentityUser<Guid>, new()
        where TRole : MongoIdentityRole<Guid>, new()
    {
        public IdentityMapper()
        {
            CreateMap<TUser, AUserDTO>()
            .ForMember(dest => dest.FullName, opt =>
                opt.MapFrom((src, _, _, context) => GetFullName(src))
            );
            CreateMap<TRole, ARoleDTO>();

        }

        private static string GetFullName(TUser user)
        {
            if (user == null) return string.Empty;

            var firstName = user.GetType().GetProperty("FirstName")?.GetValue(user)?.ToString() ?? "";
            var lastName = user.GetType().GetProperty("LastName")?.GetValue(user)?.ToString() ?? "";

            return $"{firstName} {lastName}".Trim();
        }
    }
}
