using HUBT_Social_Base.Service;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Base.ASP_Extentions;

namespace HUBT_Social_Chat_Service.Services
{
    public class UserService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IUserService
    {
        public async Task<List<AUserDTO>> GetUser()
        {
            string path = $"api/identity/userAll";
            var _ = await SendRequestAsync(path, ApiType.GET);
            return _.ConvertTo<List<AUserDTO>>()??new List<AUserDTO>();
        }
        public async Task<AUserDTO?> GetUserById(string? userId)
        {
            if (userId == null) { return  null; }
            List<AUserDTO> users = await GetUser();
            AUserDTO? target = users.Where(u => u.Id.ToString() == userId).FirstOrDefault();
            return target;
        }
    }
}