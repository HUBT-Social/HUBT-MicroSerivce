using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using HUBT_Social_Core.Settings.@enum;
using System;
using System.Globalization;
using User_API.Src.Service;

namespace User_API.Src.Models
{
    public class UserTimetableOutput(ITempService tempService)
    {
        private readonly ITempService _tempService = tempService;
        private DateTime _starttime;
        private DateTime _endtime;
        public string VersionKey { get; set; } = string.Empty;

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

        public List<TimetableOutputDTO> ReformTimetables { get; set; } = [];

        public async Task<List<TimetableOutputDTO>> GenerateReformTimetables(List<CouresDTO> couresDTOs)
        {
            DateTime currentDate = Starttime;
            while (currentDate <= Endtime)
            {
                foreach (var couresDTO in couresDTOs)
                {

             
                    if (IsMatchingDay(couresDTO.TimeTableDTO.Day, currentDate))
                    {
                                                
                        ReformTimetable reformTimetable = new(couresDTO.TimeTableDTO, currentDate);
                        TimetableOutputDTO timetableOutputDTO = reformTimetable;
                        timetableOutputDTO.CourseId = couresDTO.Id;
                        TimetableOutputDTO result = await _tempService.StoreIn(timetableOutputDTO);
                        if (result.Id != string.Empty)
                        {
                            reformTimetable.Id = result.Id;
                            ReformTimetables.Add(reformTimetable);
                        }
                        
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

    public class ReformTimetable : TimetableOutputDTO
    {

        public ReformTimetable(TimeTableDTO timetable, DateTime startDay)
        {
            this.Id = timetable.Id;
            this.ClassName = timetable.ClassName;
            this.StartTime = SetStartTime(timetable.Session, startDay);
            this.EndTime = Type == TimeTableType.Study ? StartTime.AddHours(5) :null;
            this.Room = timetable.Room;
            this.Subject = timetable.Subject;
            this.ZoomID = timetable.ZoomID;
            this.Type = TimeTableType.Study;
        }

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


}

