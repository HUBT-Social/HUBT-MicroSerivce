using HUBT_Social_Core.Models;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_Core
{
    public class CoreController : ControllerBase
    {
        /// <summary>
        /// Override phương thức Ok để định dạng phản hồi chuẩn.
        /// </summary>
        [NonAction]
        public override OkObjectResult Ok(object? value)
        {
            return base.Ok(new ResponseDTO
            {
                Message = value is string message ? message : "Request succeeded.",
                Data = value is string ? null : value,
                StatusCode = System.Net.HttpStatusCode.OK
            });
        }

        /// <summary>
        /// Override phương thức BadRequest để định dạng phản hồi lỗi chuẩn.
        /// </summary>
        [NonAction]
        public override BadRequestObjectResult BadRequest(object? value)
        {
            return base.BadRequest(new ResponseDTO
            {
                Message = value is string message ? message : "Bad request.",
                Data = value is string ? null : value,
                StatusCode = System.Net.HttpStatusCode.BadRequest
            });
        }

        /// <summary>
        /// Override phương thức Unauthorized để định dạng phản hồi không xác thực chuẩn.
        /// </summary>
        [NonAction]
        public override UnauthorizedObjectResult Unauthorized(object? value)
        {
            return base.Unauthorized(new ResponseDTO
            {
                Message = value is string message ? message : "Unauthorized access.",
                Data = value is string ? null : value,
                StatusCode = System.Net.HttpStatusCode.Unauthorized
            });
        }

        /// <summary>
        /// Phương thức phản hồi NotFound được tùy chỉnh.
        /// </summary>
        [NonAction]
        public override NotFoundObjectResult NotFound(object? value)
        {
            return base.NotFound(new ResponseDTO
            {
                Message = value is string message ? message : "Resource not found.",
                Data = value is string ? null : value,
                StatusCode = System.Net.HttpStatusCode.NotFound
            });
        }

    }
}
