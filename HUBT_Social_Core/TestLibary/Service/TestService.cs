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

namespace TestLibary.Service
{
    public class TestService(IHttpService httpService, string basePath) : BaseService(httpService, basePath), ITestService
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

        public async Task<List<ScoreDTO>?> GetStudentScoreByMasv(string masv)
        {
            string path = $"sinhvien/diem?masv={masv}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<ScoreDTO>>(); ;
        }

        public async Task<List<TimeTableDTO>?> GetTimeTableByClassName(string className)
        {
            string path = $"diemtb?className={className}";
            ResponseDTO response = await SendRequestAsync(path, ApiType.GET);
            return response.ConvertTo<List<TimeTableDTO>>();
        }
    }
}
