﻿using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace HUBT_Social_Base.Service
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> SendAsyncCore(RequestDTO request);
        Task<ResponseDTO> SendAsync(RequestDTO request);

    }
}
