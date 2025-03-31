using System;
using System.Collections.Generic;
using System.Linq;
namespace HUBT_Social_Core.ASP_Extensions
{
    public static class UrlExtension
    {
        public static string BuildUrl(this string baseUrl, Dictionary<string, object>? queryParams = null)
        {
            if (queryParams == null || queryParams.Count == 0)
            {
                return baseUrl;
            }

            var queryString = string.Join("&", queryParams
                .Where(kv => kv.Value != null) // Bỏ qua tham số null
                .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value.ToString())}"));

            return $"{baseUrl}?{queryString}";
        }
    }

}

