namespace User_API.Src.Models
{
    public class UserCourse : Course
    {
        public int SubjectYear { get; set; }
    }

    public class Course
    {
        public string Major { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public int SubjectCredit { get; set; }
    }

    public class OutPutCourse
    {
        public List<Course> Courses { get; set; } = [];
        public string Year { get; set; } = string.Empty;
    }

    public static class CourseExtensions
    {
        public static List<OutPutCourse> FormatOutput(this List<UserCourse> courses)
        {
            return courses
                .GroupBy(c => c.SubjectYear)
                .Select(g => new OutPutCourse
                {
                    Year = $"{g.Key} - {g.Key + 1}",
                    Courses = g.Select(c => new Course
                    {
                        Major = c.Major,
                        SubjectName = c.SubjectName,
                        SubjectCredit = c.SubjectCredit
                    }).ToList()
                })
                .ToList();
        }
    }
}
