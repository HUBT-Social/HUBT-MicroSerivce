using HUBT_Social_Base.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;


namespace HUBT_Social_Base.Helpers
{
    public static class ServerHelper
    {
        public static readonly HttpClient client = new();
        public static string? GetIPAddress(HttpContext context)
        {
            string forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();

            if (!string.IsNullOrEmpty(forwardedFor))
            {
                string ip = forwardedFor.Split(',')[0].Trim();
                return ip;
            }

            IPAddress? remoteIp = context.Connection.RemoteIpAddress;

            if (remoteIp != null)
            {
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }
                if (IPAddress.IsLoopback(remoteIp))
                {
                    remoteIp = IPAddress.Parse("192.168.1.1");
                }
                return remoteIp.ToString();
            }

            return null;
        }
        public static async Task<string> GetLocationFromIpAsync(string ipAddress)
        {
            string apiKey = "3810e9eac14f17"; // Thay YOUR_API_KEY bằng API key của bạn
            string url = $"http://ipinfo.io/{ipAddress}/json?token={apiKey}";

            try
            {
                var response = await client.GetStringAsync(url);
                var locationData = JsonConvert.DeserializeObject<LocationResponseBaseDTO>(response);

                if (locationData != null)
                    return $"{locationData.City},<br> {locationData.Region}, {locationData.Country}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return $"";
        }
        public static string ConvertToCustomString(DateTime dateTime)
        {
            // Định dạng chuyển đổi thời gian thành chuỗi
            string formattedDateTime = dateTime.ToString("dddd, dd MMMM yyyy 'at' hh:mm:ss tt");
            return formattedDateTime;
        }

    }
}
