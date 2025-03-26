using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.OutSourceDataDTO;
using System;
using System.Collections.Generic;
using User_API.Src.Service;

namespace User_API.Src.Models
{
    public class TimetableInfo(TimeTableDTO timetable,
        List<AUserDTO> studentDTOs,
        DateTime stattingTime) : ReformTimetable(timetable, stattingTime)
    {
        private static readonly Random _random = new();
        public int CountMenber { get; set; } = studentDTOs.Count;
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
