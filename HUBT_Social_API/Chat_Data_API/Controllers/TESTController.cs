using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace Chat_Data_API.Controllers
{
    [Route("api/test")]
    [ApiController]
    [Authorize]
    public class TESTController(HUBT_Social_Base.Service.ICloudService clouldService) : ControllerBase
    {
        public readonly HUBT_Social_Base.Service.ICloudService _clouldService = clouldService;



        [HttpPost("upfile")]
        public async Task<IActionResult> UpFile(List<IFormFile> files)
        {
            if(files is null)
            {
                return BadRequest("File null.");
            }
            var result = await _clouldService.UploadFilesAsync(files);
            return Ok(result);
        }
    }
}
