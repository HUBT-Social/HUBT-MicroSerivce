using AutoMapper;
using CloudServiceCenter.src.HelperFile;
using HUBT_Social_Base;
using HUBT_Social_Core;
using HUBT_Social_Core.Models.Requests.Cloud;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CloudServiceCenter.src.Controller
{
    [Route("api/cloudinary")]
    [ApiController]
    public class UploadController(HUBT_Social_Base.Service.ICloudService clouldService) : CoreController
    {
        public readonly HUBT_Social_Base.Service.ICloudService _clouldService = clouldService;



        [HttpPost("upfiles")]
        public async Task<IActionResult> UpFiles(List<IFormFile> files)
        {
            if (files is null)
            {
                return BadRequest("Files null.");
            }
            var result = await _clouldService.UploadFilesAsync(files);
            return Ok(result);
        }

        [HttpPost("upfile")]
        public async Task<IActionResult> UpFile(IFormFile file)
        {
            if (file is null)
            {
                return BadRequest("File null.");
            }
            var result = await _clouldService.UploadFileAsync(file);
            return Ok(result);
        }

        [HttpPost("upload-base64-files")]
        public async Task<IActionResult> UpFiles([FromBody] List<UploadBase64Request> request)
        {
            if (request==null || request.Count ==0)
                return BadRequest("Invalid input.");
            List<IFormFile> files = new List<IFormFile>();
            foreach (var req in request)
            {
                var formFile = FileHelper.Base64ToFormFile(req.Base64String, req.FileName);
                if(formFile != null) {  files.Add(formFile); }
            }
            if(files.Count == 0) { return BadRequest(); }
            var result = await _clouldService.UploadFilesAsync(files);
            return Ok(result);
        }
        [HttpPost("upload-base64-file")]
        public async Task<IActionResult> UpFile([FromBody] UploadBase64Request request)
        {
            if (string.IsNullOrEmpty(request.Base64String) || string.IsNullOrEmpty(request.FileName))
                return BadRequest("Invalid input.");
            var formFile = FileHelper.Base64ToFormFile(request.Base64String, request.FileName);
            if(formFile == null) { return BadRequest(); }
            var result = await _clouldService.UploadFileAsync(formFile);
            return Ok(result);
        }
        [HttpPost("convert-to-base64")]
        public async Task<IActionResult> ConvertToBase64(IFormFile file)
        {
            var result = await FileHelper.FormFileToBase64ObjectAsync(file);
            if (result == null)
                return BadRequest("File is invalid or empty.");

            return Ok(result);
        }
    }
}
