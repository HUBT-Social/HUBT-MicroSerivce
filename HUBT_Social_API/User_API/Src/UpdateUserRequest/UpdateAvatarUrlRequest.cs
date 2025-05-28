using HUBT_Social_Core.Models.Requests.Cloud;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace User_API.Src.UpdateUserRequest;

public class UpdateAvatarRequest
{
    //public UploadBase64Request? File { get; set; }
    [Required]
    public IFormFile? File {  get; set; }

}