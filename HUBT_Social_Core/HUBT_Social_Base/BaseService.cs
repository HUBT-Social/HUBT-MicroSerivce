using HUBT_Social_Base.Service;
using HUBT_Social_Core;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_Base
{
    public abstract class BaseService(IHttpService httpService, string basePath) : IBaseService
    {
        protected readonly IHttpService _httpService = httpService;
        public string BasePath { get; private set; } = basePath;

        protected async Task<ResponseDTO> SendRequestAsync(string endpoint, ApiType apiType, object? data = null,string? accessToken=null)
        {
            var request = new RequestDTO
            {
                Url = $"{BasePath.TrimEnd('/')}/{endpoint.TrimStart('/')}",
                ApiType = apiType,
                Data = data,
                AccessToken = accessToken
            };

            return await _httpService.SendAsync(request);
        }
    }

    public interface IBaseService
    {
        string BasePath { get; }
    }
}
