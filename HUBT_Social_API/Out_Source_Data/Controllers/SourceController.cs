using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Out_Source_Data.Datas;
using Out_Source_Data.Models;

namespace Out_Source_Data.Controllers
{
    [Route("api/source")]
    [ApiController]
    public class SourceController(StudentDB context) : ControllerBase
    {
        private readonly StudentDB _context = context;

        [HttpGet("student")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents([FromQuery] string? masv)
        {
            try
            {
                if (!string.IsNullOrEmpty(masv))
                {
                    var sinhVien = await _context.Students.FirstOrDefaultAsync(s => s.MASV == masv);
                    if (sinhVien == null) return NotFound(new { message = "Không tìm thấy sinh viên" });
                    return Ok(sinhVien);
                }

                
                return Ok(await _context.Students.Take(100).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi truy vấn sinh viên", error = ex.Message });
            }
        }

        [HttpGet("timetable")]
        public async Task<ActionResult<IEnumerable<TimeTable>>> GetTimeTables([FromQuery] string? className)
        {
            try
            {
                if (!string.IsNullOrEmpty(className))
                {
                    var timeTables = await _context.TimeTables.Where(t => t.ClassName == className).ToListAsync();
                    if (timeTables == null || timeTables.Count == 0)
                        return NotFound(new { message = "Không tìm thấy thời khóa biểu" });

                    return Ok(timeTables);
                }
                return Ok(await _context.TimeTables.Take(100).ToListAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi truy vấn thời khóa biểu", error = ex.Message });
            }
        }


    }
}
