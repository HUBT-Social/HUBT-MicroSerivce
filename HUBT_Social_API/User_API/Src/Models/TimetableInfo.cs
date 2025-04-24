using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using System;
using System.Collections.Generic;
using User_API.Src.Service;

namespace User_API.Src.Models
{
    public class TimetableInfo : TimetableOutputDTO
    {
        private readonly List<AUserDTO> studentDTOs;
        public TimetableInfo(TimetableOutputDTO timetable,
            List<AUserDTO> studentDTOs)
        {
            this.Id = timetable.Id;
            this.ClassName = timetable.ClassName;
            this.StartTime = timetable.StartTime;
            this.EndTime = timetable.EndTime;
            this.Subject = timetable.Subject;
            this.Room = timetable.Room;
            this.ZoomID = timetable.ZoomID;
            this.Type = timetable.Type;
            this.studentDTOs = studentDTOs;
        }
        private static readonly Random _random = new();
        public int CountMenber { get => studentDTOs.Count; }
        public int CreditNum => _random.Next(0, 2) == 0 ? 2 : 4;

        public List<TimetableMember> TimetableMembers => GenderMembers();
        private List<TimetableMember> GenderMembers()
        {
            List<TimetableMember> members = [];
            foreach (var student in studentDTOs)
            {
                members.Add(new TimetableMember
                {
                    AvatarUrl = student.AvataUrl,
                    UserName = student.UserName,
                    FullName = $"{student.LastName} {student.FirstName}"
                });
            }
            return members;
        }
    }
    public class TimetableMember
    {
        public string AvatarUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
