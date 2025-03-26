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
            return response.ConvertTo<List<TimeTableDTO>>();
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

