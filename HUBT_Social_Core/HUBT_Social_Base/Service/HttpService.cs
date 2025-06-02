using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Authentication.BearerToken;
using HUBT_Social_Base.ASP_Extentions;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace HUBT_Social_Base.Service
{
    public class HttpService(IHttpClientFactory httpClientFactory) : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        public async Task<ResponseDTO> SendAsync(RequestDTO request)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                HttpRequestMessage message = CreateHttpRequestMessage(request);
                HttpResponseMessage apiResponse = await client.SendAsync(message);

                return await ProcessApiResponse(apiResponse);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return HandleException();
            }
        }
        public async Task<HttpResponseMessage> SendAsyncCore(RequestDTO request)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient();
                HttpRequestMessage message = CreateHttpRequestMessage(request);
                HttpResponseMessage apiResponse = await client.SendAsync(message);

                return apiResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("An error occurred while processing your request. Please try again.")
                };
            }
        }


        private static HttpRequestMessage CreateHttpRequestMessage(RequestDTO request)
        {
            var message = new HttpRequestMessage
            {
                RequestUri = new Uri(request.Url),
                Method = GetHttpMethod(request.ApiType)
            };

            message.Headers.Add("Accept", "application/json");

            if (!string.IsNullOrEmpty(request.AccessToken))
            {
                message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", request.AccessToken);
            }

            if (request.Data != null)
            {
                if (request.Data is MultipartFormDataContent multipartContent)
                {
                    // Nếu Data đã là MultipartFormDataContent (gửi file)
                    message.Content = multipartContent;
                }
                else
                {
                    // Gửi JSON bình thường
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(request.Data),
                        Encoding.UTF8,
                        "application/json"
                    );
                }
            }

            return message;
        }

        private static HttpMethod GetHttpMethod(ApiType apiType) => apiType switch
        {
            ApiType.POST => HttpMethod.Post,
            ApiType.PUT => HttpMethod.Put,
            ApiType.DELETE => HttpMethod.Delete,
            _ => HttpMethod.Get,
        };

        private static async Task<ResponseDTO> ProcessApiResponse(HttpResponseMessage apiResponse)
        {
            return apiResponse.StatusCode switch
            {
                HttpStatusCode.OK => await HandleSuccessResponse(apiResponse),
                HttpStatusCode.Created => await HandleSuccessResponse(apiResponse),
                HttpStatusCode.NotFound => await HandleSuccessResponse(apiResponse),
                HttpStatusCode.Unauthorized => await HandleSuccessResponse(apiResponse),
                HttpStatusCode.Forbidden => await HandleSuccessResponse(apiResponse),
                HttpStatusCode.BadRequest => await HandleSuccessResponse(apiResponse),
                HttpStatusCode.InternalServerError => await HandleSuccessResponse(apiResponse),
                _ => CreateErrorResponse(KeyStore.ApiError)
            };
        }
        private static async Task<ResponseDTO> HandleSuccessResponse(HttpResponseMessage response)
        {
            return await response.ConvertTo<ResponseDTO>()
                   ?? CreateErrorResponse(KeyStore.ApiError);
        }


        private static ResponseDTO CreateErrorResponse(string key) => new()
        {
            Message = LocalValue.Get(key),
            StatusCode = HttpStatusCode.BadRequest,
        };

        private static ResponseDTO HandleException()
        {
            return CreateErrorResponse(KeyStore.ApiError);
        }
    }
}
