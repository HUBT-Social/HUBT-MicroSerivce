using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/tempCourse")]
    [ApiController]
    public class TempCourseController(
        IMongoService<TempCourse> tempCourse,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempCourse> _tempCourse = tempCourse;

        [HttpGet]
        public async Task<IActionResult> Get(int page)
        {
            if (page>=0)
            {
                Console.WriteLine("So ban gi: ", _tempCourse.Count());
                var filter = Builders<TempCourse>.Filter.Eq(c => c.RoomCreated, false);
                var tempListCourse = await _tempCourse.GetSlide(page, 10, filter);
                if (!tempListCourse.Any()) { return BadRequest(); };
               
                List<CreateGroupByCourse> results = [];
                foreach (var course in tempListCourse)
                {
                    CreateGroupByCourse item = new()
                    {
                        Id = course.Id,
                        ClassName = course.TimeTableDTO.ClassName, 
                        Subject = course.TimeTableDTO.Subject,
                        ListUserNames = [.. course.StudentIDs],
                    };
                    results.Add(item);
                }
                return Ok(results);

            }
            return BadRequest(LocalValue.Get(KeyStore.InvalidInformation));
        }
        [HttpPut("status")]
        public async Task<ActionResult> Put([FromQuery] string courseId)
        {
            try
            {
                Expression<Func<TempCourse, bool>> filter = c => c.Id == courseId;
                var update = Builders<TempCourse>.Update.Set(c => c.RoomCreated, true);
                bool updated = await _tempCourse.UpdateByFilter(filter, update);
                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(BaseOk(ex));
                return BadRequest();
            }
        }
    }
}
//                          