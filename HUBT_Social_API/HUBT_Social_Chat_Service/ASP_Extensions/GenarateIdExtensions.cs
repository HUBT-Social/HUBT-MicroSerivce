using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.ASP_Extensions
{
    public static class GenarateIdExtensions
    {
        public static string ConvertToId(this string groupName)
        {
            string normalizedGroupName = new string(groupName
                .Normalize(NormalizationForm.FormD)
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark && c <= 127)
                .ToArray())
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant();

            normalizedGroupName = Regex.Replace(normalizedGroupName, @"\s+", ".");
            normalizedGroupName = Regex.Replace(normalizedGroupName, @"[^a-z0-9\.]", string.Empty);

            Random random = new Random();
            string randomPart = random.Next(10000, 99999).ToString();

            return $"@{normalizedGroupName}.{randomPart}";
        }
    }
    
}
