using HUBT_Social_Base;
using HUBT_Social_Base.Service;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Settings.@enum;
using Microsoft.AspNetCore.Mvc;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using Amazon.Runtime.Internal;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Base.ASP_Extentions;
using System.Collections.Generic;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.ASP_Extensions;
using Amazon.Runtime.Internal.Transform;
using HUBT_Social_Core.Models.DTOs.UserDTO;

namespace User_API.Src.Service
{
    public class OutSourceService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IOutSourceService
    {
        public async Task<AVGScoreDTO?> GetAVGScoreByMasv(string masv)
        {
            string path = KeyStore.OutSourceUrls.Get_StudentAvgScore
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"masv",masv }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<AVGScoreDTO>();
        }

        public async Task<StudentDTO?> GetStudentByMasv(string masv)
        {

            string path = KeyStore.OutSourceUrls.Get_StudentData
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"masv",masv }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<StudentDTO>();
        }
        public async Task<List<StudentDTO>> GetStudentByClassName(string className)
        {
            string path = KeyStore.OutSourceUrls.Get_StudentList
                .Replace("{className}", className);
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<StudentDTO>>() ?? [];
        }

        public async Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv)
        {
            string path = KeyStore.OutSourceUrls.Get_StudentScoreByRoute
                .Replace("{masv}", masv);
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<ScoreDTO>>();
        }

        public async Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className)
        {

            string path = KeyStore.OutSourceUrls.Get_StudentTimeTable
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"className",className }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            
            return response.ConvertTo<List<TimeTableDTO>>();
        }
        public async Task<List<SubjectDTO>?> GetCouresAsync(string className)
        {
            string[] paths = className.Split(".");
            string major = new(paths[0].TakeWhile(char.IsLetter).ToArray());
            string course = new(paths[0].SkipWhile(char.IsLetter).TakeWhile(char.IsDigit).ToArray());


            string path = KeyStore.OutSourceUrls.Get_Subject
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"major",major },
                        {"course",course }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);

            List<SubjectDTO>? couresDTOs = response.ConvertTo<List<SubjectDTO>>();
            
            if (couresDTOs?.Count > 0)
                return couresDTOs;
            return null;
        }

        public async Task<TimeTableDTO?> GetTimeTableById(string id)
        {

            string path = KeyStore.OutSourceUrls.Get_StudentTimeTable
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"id",id }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            var timeTableList = response.ConvertTo<List<TimeTableDTO>>();
            return timeTableList?.FirstOrDefault();
        }
        public async Task<List<StudentClassName>> GetSlideStudentClassName(int page)
        {
            string path = KeyStore.OutSourceUrls.Get_Slice_Students
                .BuildUrl(
                    new Dictionary<string, object>
                    {
                        {"page",page.ToString() }
                    }
                );
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            var slice = response.ConvertTo<List<StudentClassName>>();
            return slice??new List<StudentClassName>();
        }
    }
}

