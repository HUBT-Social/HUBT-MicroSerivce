using HUBT_Social_Core.Settings.@enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Models.DTOs.UserDTO
{
    public class TimetableOutputDTO
    {
        private string _id = string.Empty;
        private string _className = string.Empty;
        private string _subject = string.Empty;
        private string _room = string.Empty;
        private string? _zoomID = string.Empty;
        private DateTime _starttime;
        private DateTime? _endtime;
        public string Id
        {
            get => _id;
            set => _id = value.ToLower();
        }

        public string ClassName
        {
            get => _className;
            set => _className = value.ToLower();
        }

        public DateTime StartTime
        {
            get => _starttime;
            set
            {
                _starttime = value;
            }
        }

        public DateTime? EndTime
        {
            get => _endtime;
            set
            {
                if (value <= _starttime)
                    throw new ArgumentException("Endtime must be later than Starttime.");
                _endtime = value;
            }
        }

        public string Subject
        {
            get => _subject;
            set => _subject = value.ToLower();
        }

        public string Room
        {
            get => _room;
            set => _room = value.ToLower();
        }

        public string? ZoomID
        {
            get => _zoomID;
            set => _zoomID = value?.ToLower();
        }
        public string CourseId { get; set; } = string.Empty;
        public TimeTableType Type { get; set; } = TimeTableType.Study;
    }
}
