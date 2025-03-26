using AutoMapper;
using HUBT_Social_Base;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Settings;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TempRegister_API.Src.Models;

namespace TempRegister_API.Src.Controllers
{
    [Route("api/temptimetable")]
    [ApiController]
    public class TempTimetableController(
        IMongoService<TempTimetable> tempTimeTable,
        IOptions<JwtSetting> option,
        IMapper mapper) : DataLayerController(mapper, option)
    {
        private readonly IMongoService<TempTimetable> _tempTimeTable = tempTimeTable;
        
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] string id)
        {
            TempTimetable? timetable = await _tempTimeTable.GetById(id);
            if (timetable == null)
            {
                return NotFound(new { message = "Timetable not found" });
            }
            TimetableOutputDTO timetableOutputDTO = _mapper.Map<TimetableOutputDTO>(timetable);
            return Ok(timetableOutputDTO);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TimetableOutputDTO timetableOutDTO)
        {
            timetableOutDTO.Id = string.Empty;
            TempTimetable timetable = _mapper.Map<TempTimetable>(timetableOutDTO);

            if (await _tempTimeTable.Create(timetable))
            {
                timetableOutDTO.Id = timetable.Id;
                return Ok(timetableOutDTO);
            }
            return BadRequest(LocalValue.Get(KeyStore.UnableToStoreInDatabase));
        }
    }
}
