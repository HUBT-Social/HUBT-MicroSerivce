using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.EmailDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_Email_Service.Services;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Org.BouncyCastle.Crypto;
using Postcode_API.Src.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Postcode_API.Src.Controllers
{
    [Route("api/postcode")]
    [ApiController]
    public class PostcodeController(
        IMongoService<Postcode> postcode,
        IOptions<JwtSetting> option,
        IEmailService emailService,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<Postcode> _postcodeService = postcode;
        private readonly IEmailService _emailService = emailService;
        [HttpPost("send-postcode")]
        public async Task<IActionResult> SendPostcodeAsync(EmailRequest request)
        {
            if (!ModelState.IsValid) 
                return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));

            if(await _emailService.SendEmailAsync(request))
                return Ok(LocalValue.Get(KeyStore.OtpSent));

            return BadRequest(LocalValue.Get(KeyStore.UnableToSendOTP));
        }
        [HttpPost("create-postcode")]
        public async Task<IActionResult> CreatePostcodeAsync(CreatePostcodeRequest request)
        {
            var code = GenerateOtp();

            Postcode? postcode = await _postcodeService.Find(
                pc => pc.IPAddress == request.IpAddress && pc.UserAgent == request.UserAgent
                ).FirstOrDefaultAsync();

            if (postcode is not null)
            {
                postcode.Code = code;
                postcode.ExpireTime = DateTime.UtcNow;
                postcode.Email = request.Receiver;

                if (await _postcodeService.Update(postcode))
                {
                    PostCodeDTO codeDTO = _mapper.Map<PostCodeDTO>(postcode);
                    return Ok(postcode);
                }
            };
            try
            {
                Postcode newPostcode = new()
                {
                    UserAgent = request.UserAgent,
                    IPAddress = request.IpAddress,
                    Code = code,
                    Email = request.Receiver,
                    ExpireTime = DateTime.UtcNow
                };

                if (await _postcodeService.Create(newPostcode))
                {
                    PostCodeDTO codeDTO = _mapper.Map<PostCodeDTO>(newPostcode);
                    return Ok(codeDTO);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));

        }
        [HttpGet("current-postcode")]
        public async Task<IActionResult> GetCurrentPostCode([FromQuery] PostcodeRequest request)
        {
            Postcode? postcode = await _postcodeService
                .Find(ps => ps.UserAgent == request.UserAgent && ps.IPAddress == request.IpAddress)
                .FirstOrDefaultAsync();

            if (postcode != null) return Ok(postcode);

            return BadRequest(LocalValue.Get(KeyStore.NoMessagesFound));
        }

        private static string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
