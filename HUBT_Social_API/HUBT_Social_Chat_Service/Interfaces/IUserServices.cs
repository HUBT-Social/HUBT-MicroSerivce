using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Interfaces
{
    public interface IUserService : IBaseService
    {
        Task<List<AUserDTO>> GetUser();
        Task<AUserDTO?> GetUserById(string? userId);
    }
    
}
