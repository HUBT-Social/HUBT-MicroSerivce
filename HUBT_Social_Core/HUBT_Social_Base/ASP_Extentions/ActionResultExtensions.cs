﻿using HUBT_Social_Core.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HUBT_Social_Base.ASP_Extentions
{
    public static class ActionResultExtensions
    {
        public static T? ConvertTo<T>(this ResponseDTO result) where T : class
        {
            if (result != null)
            {
                var value = result.Data;

                try
                {
                    var json = JsonConvert.SerializeObject(value);
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (JsonException)
                {
                    return null;
                }
                
            }
            return null;
        }
    }
}
