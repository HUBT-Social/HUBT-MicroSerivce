using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/tempRegister")]
    [ApiController]
    public class TempRegisterController(
        IMongoService<TempUserRegister> tempUserRegister,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempUserRegister> _tempUserRegister = tempUserRegister;

        [HttpPost]
        public async Task<IActionResult> StoreTempUser([FromBody] RegisterRequest model)
        {
            if (model == null) return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));


            TempUserRegister? tempUserRegister = await _tempUserRegister.GetById(model.Email);
            try
            {
                if (tempUserRegister == null)
                {
                    TempUserRegister tempUser = new()
                    {
                        Email = model.Email,
                        ExpireTime = DateTime.UtcNow,
                        UserName = model.UserName,
                        Password = model.Password
                    };
                    await _tempUserRegister.Create(tempUser);
                    return Ok(LocalValue.Get(KeyStore.StepOneVerificationSuccess));
                };
                tempUserRegister.UserName = model.UserName;
                tempUserRegister.Password = model.Password;
                tempUserRegister.ExpireTime = DateTime.UtcNow;

                await _tempUserRegister.Update(tempUserRegister);
                return Ok(LocalValue.Get(KeyStore.StepOneVerificationSuccess));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
            }
        }
        [HttpGet]
        public async Task<IActionResult> Get(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                TempUserRegister? tempUser = await _tempUserRegister.GetById(email);
                return tempUser != null ? Ok(_mapper.Map<TempUserDTO>(tempUser)) : BadRequest(LocalValue.Get(KeyStore.UserNotFound));
            }
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        }
    }
}
