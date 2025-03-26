using HUBT_Social_Core.Models.OutSourceDataDTO;
using System;
using System.Globalization;

namespace User_API.Src.Models
{
    public class UserTimetableOutput
    {
        private DateTime _starttime;
        private DateTime _endtime;

        public DateTime Starttime
        {
            get => _starttime;
            set
            {
                _starttime = value;
            }
        }

        public DateTime Endtime
        {
            get => _endtime;
            set
            {
                if (value <= _starttime)
                    throw new ArgumentException("Endtime must be later than Starttime.");
                _endtime = value;
            }
        }

        public List<ReformTimetable> ReformTimetables { get; set; } = [];

        public List<ReformTimetable> GenerateReformTimetables(List<TimeTableDTO> timetables)
        {
            
            DateTime currentDate = Starttime;
            while (currentDate <= Endtime)
            {
                foreach (var timetable in timetables)
                {
                    if (IsMatchingDay(timetable.Day, currentDate))
                    {
                        ReformTimetables.Add(new ReformTimetable(timetable, currentDate));
                    }
                }
                currentDate = currentDate.AddDays(1);
            }
            return ReformTimetables;
        }
        private static bool IsMatchingDay(string day, DateTime date)
        {
            DayOfWeek dayOfWeek = day.ToLower() switch
            {
                "2" => DayOfWeek.Monday,
                "3" => DayOfWeek.Tuesday,
                "4" => DayOfWeek.Wednesday,
                "5" => DayOfWeek.Thursday,
                "6" => DayOfWeek.Friday,
                "7" => DayOfWeek.Saturday,
                "cn" => DayOfWeek.Sunday,
                _ => throw new ArgumentException("Invalid day value. Expected values are '2', '3', '4', '5', '6', '7', or 'cn'."),
            };
            return date.DayOfWeek == dayOfWeek;
        }
    }

    public class ReformTimetable(TimeTableDTO timetable, DateTime startDay)
    {

        public string Id => timetable.Id;
        public string ClassName => timetable.ClassName;
        public DateTime StartTime { get => SetStartTime(timetable.Session, startDay);}
        public DateTime? EndTime { get => Type == TimeTableType.Study ? StartTime.AddHours(5): null; }
        public string Room => timetable.Room;
        public string Subject => timetable.Subject;
        public string? ZoomID => timetable.ZoomID;
        public TimeTableType Type { get; set; } = TimeTableType.Study;

        private static DateTime SetStartTime(string session, DateTime baseDate)
        {
            DateTime startTime = session.ToLower() switch
            {
                "sáng" => baseDate.AddHours(7).AddMinutes(30),
                "chiều" => baseDate.AddHours(13),
                "tối" => baseDate.AddHours(18),
                _ => throw new ArgumentException("Invalid session value. Expected values are 'Sáng', 'Chiều', or 'Tối'."),
            };
            return startTime;
        }

    }

    public enum TimeTableType
    {
        Study,
        Exam,
        Seminal,
        RetakeExam
    }
}

