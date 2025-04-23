using Chat_Data_API.Src.Model;
using Chat_Data_API.Src.Models;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;

namespace Chat_Data_API.Src.Service
{
    public class OutDataService(
            IMongoService<ThoiKhoaBieu> timeTable,
            IMongoService<SinhVien> student,
            IMongoService<Course> course
        ) : IOutDataService
    {
        private readonly IMongoService<ThoiKhoaBieu> _timeTable = timeTable;
        private readonly IMongoService<SinhVien> _student = student;
        private readonly IMongoService<Course> _course = course;
        public async Task<bool> GenerateCourses()
        {
            List<ThoiKhoaBieu> timeTables = await _timeTable.GetAll().ToListAsync();
            if (timeTables.Count == 0) { return false; }
            List<SinhVien> students = await _student.GetAll().ToListAsync();
            if (students.Count == 0) { return false; }

            var groupedSubjects = timeTables
                .GroupBy(t => t.Subject)
                .ToList();
            try
            {
                foreach (var group in groupedSubjects)
                {
                    var subjectName = group.Key;
                    var classList = group.Select(t => t.ClassName).Distinct().ToList();

                    var existing = await _course.Find(c => c.SubjectName == subjectName).FirstOrDefaultAsync();
                    if (existing == null)
                    {
                        var course = new Course
                        {
                            SubjectName = subjectName,
                            SubjectCode = $"MH-{Guid.NewGuid().ToString().Substring(0, 5)}",
                            ClassList = classList
                        };

                        await _course.Create(course);
                    }
                    else
                    {
                        // nếu đã có thì cập nhật thêm lớp mới (nếu có)
                        var newClasses = classList.Except(existing.ClassList).ToList();
                        if (newClasses.Any())
                        {
                            existing.ClassList.AddRange(newClasses);
                            await _course.Update(existing);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error is: "+ex.ToString());
                return false;
            }

        }
    }
}
