using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Settings.@enum;

namespace User_API.Src.Models
{
    public class StudentClass: UserOutPut
    {
        public Gender Gender { get; set; }
        public DateTime BirthDay { get; set; }
        public StudentClass(AUserDTO aUser) {
            this.AvatarUrl = aUser.AvataUrl ?? "";
            this.UserName = aUser.UserName;
            this.FullName = $"{aUser.LastName} {aUser.FirstName}";
            Gender = aUser.Gender;
            BirthDay = aUser.DateOfBirth;
        }
        
    }
}
