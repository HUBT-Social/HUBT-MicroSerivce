using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Core;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using User_API.Src.Service;

namespace User_API.Src.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _identityService = userService;
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? name, [FromQuery] string? id,[FromQuery] string? email)
        {
            ResponseDTO result = await _identityService.GetUser();
            List<AUserDTO>? userDTO = result.ConvertTo<List<AUserDTO>>();
            if (userDTO != null && result.StatusCode == HttpStatusCode.OK)
            {

                if (!string.IsNullOrEmpty(name))
                {
                    userDTO = userDTO.Where(user => user.UserName == name).ToList();
                }
                if (!string.IsNullOrEmpty(email))
                {
                    userDTO = userDTO.Where(user => user.Email == email).ToList();
                }
                if (!string.IsNullOrEmpty(id))
                {
                    userDTO = userDTO.Where(user => user.Id.ToString() == id).ToList();
                }
                return Ok(userDTO);

            }
            if (result.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(result.Message);
            }
            return BadRequest(result.Message);

        }
    }
}
