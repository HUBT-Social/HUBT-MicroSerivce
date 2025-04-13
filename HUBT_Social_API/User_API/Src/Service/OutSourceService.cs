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

namespace User_API.Src.Service
{
    public class OutSourceService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), IOutSourceService
    {
        public async Task<AVGScoreDTO?> GetAVGScoreByMasv(string masv)
        {
            string path = $"diemtb?masv={masv}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<AVGScoreDTO>();
        }

        public async Task<StudentDTO?> GetStudentByMasv(string masv)
        {
            string path = $"sinhvien?masv={masv}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<StudentDTO>();
        }
        public async Task<List<StudentDTO>> GetStudentByClassName(string className)
        {
            string path = $"sinhvien/{className}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<StudentDTO>>() ?? [];
        }

        public async Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv)
        {
            string path = $"sinhvien/{masv}/diem";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<ScoreDTO>>();
        }

        public async Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className)
        {
            string path = $"ThoiKhoaBieu?className={className}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            string[] paths = className.Split(".");
            string major = new (paths[0].TakeWhile(char.IsLetter).ToArray());
            string course = new(paths[0].SkipWhile(char.IsLetter).TakeWhile(char.IsDigit).ToArray());

            string path2 = $"hocphan?major={major}&course={course}";
            ResponseDTO response2 = await SendRequestAsync(path2, ApiType.GET);

            List<TimeTableDTO>? timeTableDTOs = response.ConvertTo<List<TimeTableDTO>>();
            List<CouresDTO>? couresDTOs = response2.ConvertTo<List<CouresDTO>>();
            if (timeTableDTOs == null || couresDTOs == null)
                return null;

            Random random = new();
            List<TimeTableDTO>? responses = [];
            foreach (TimeTableDTO timeTableDTO in timeTableDTOs)
            {
                CouresDTO couresDTO = couresDTOs[random.Next(0,couresDTOs.Count)];
                timeTableDTO.Subject = couresDTO.Tenmon;
                responses.Add(timeTableDTO);
            }
            if (responses.Count > 0) 
                return responses;
            
            return null;
        }

        public async Task<TimeTableDTO?> GetTimeTableById(string id)
        {
            string path = $"ThoiKhoaBieu?id={id}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            var timeTableList = response.ConvertTo<List<TimeTableDTO>>();
            return timeTableList?.FirstOrDefault();
        }
    }
}

