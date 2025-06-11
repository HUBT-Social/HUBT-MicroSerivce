using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.DTOs.UserDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using HUBT_Social_Core.Models.Requests.Temp;
using System;
using System.Collections.Generic;
using User_API.Src.Service;

namespace User_API.Src.Models
{
    public class TimetableInfo : TimetableOutputDTO
    {
        private readonly List<AUserDTO> studentDTOs;
        public TimetableInfo(TimetableOutputDTO timetable,
            List<AUserDTO> studentDTOs,
            List<AUserDTO> teacherDTOs,
            string chatRoomID)
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
            this.CourseId = timetable.CourseId;
            TeacherIDs = teacherDTOs;
            ChatRoomId = chatRoomID;
        }
        private static readonly Random _random = new();
        public int CreditNum => _random.Next(0, 2) == 0 ? 2 : 4;
        public string ChatRoomId { get; set; }

        public List<AUserDTO> TeacherIDs;
        public List<UserOutPut> StudentMembers => GenderMembers(studentDTOs);

        public List<UserOutPut> TeacherleMembers => GenderMembers(TeacherIDs);

        private static List<UserOutPut> GenderMembers(List<AUserDTO> aUsers)
        {
            List<UserOutPut> members = [];
            foreach (var student in aUsers)
            {
                members.Add(new UserOutPut
                {
                    AvatarUrl = student.AvataUrl ?? "",
                    UserName = student.UserName,
                    FullName = $"{student.LastName} {student.FirstName}"
                });
            }
            return members;
        }
    }
    public class UserOutPut
    {
        public string AvatarUrl { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
