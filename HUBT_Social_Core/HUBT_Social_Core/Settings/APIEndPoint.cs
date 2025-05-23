using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Core.Settings
{
    public static class APIEndPoint
    {
        public static class ChatDataUrls
        {
            private const string EndPoint_Chat_Data_Chat = "api/chat";
            private const string EndPoint_Chat_Data_Room = "api/room";

            public static readonly string Post_Create_Group = $"{EndPoint_Chat_Data_Chat}/create-group";
            public static readonly string Delete_Group = $"{EndPoint_Chat_Data_Chat}/delete-group";
            public static readonly string Get_Search_Group = $"{EndPoint_Chat_Data_Chat}/search-groups";
            public static readonly string Get_All_Group = $"{EndPoint_Chat_Data_Chat}/get-all-rooms";
            public static readonly string Get_User_Group = $"{EndPoint_Chat_Data_Chat}/get-rooms-of-user";
            public static readonly string Put_Update_Group_Name = $"{EndPoint_Chat_Data_Room}/update-group-name";
            public static readonly string Put_Update_Group_Avatar = $"{EndPoint_Chat_Data_Room}/update-avatar";
            public static readonly string Put_Update_Nick_Name = $"{EndPoint_Chat_Data_Room}/update-nickname";
            public static readonly string Put_Update_Participant_Role = $"{EndPoint_Chat_Data_Room}/update-participant-role";
            public static readonly string Post_Join_Group = $"{EndPoint_Chat_Data_Room}/join-room";
            public static readonly string Post_Kick_Member = $"{EndPoint_Chat_Data_Room}/kick-member";
            public static readonly string Post_Leave_Group = $"{EndPoint_Chat_Data_Room}/leave-room";
            public static readonly string Get_History_Content = $"{EndPoint_Chat_Data_Room}/message-history";
            public static readonly string Get_Members = $"{EndPoint_Chat_Data_Room}/get-members";
        }

        public static class IdentityUrls
        {
            private const string EndPoint_Identity = "api/identity";

            public static readonly string Post_Login = $"{EndPoint_Identity}/auth/vertifile-account";
            public static readonly string Post_SignUp = $"{EndPoint_Identity}/auth/create-account";
            public static readonly string Get_All_User = EndPoint_Identity;
            public static readonly string Get_User_In_List_User_Name = $"{EndPoint_Identity}/users-in-list-userName";
            public static readonly string Get_Current_User = $"{EndPoint_Identity}/user";
            public static readonly string Get_User_From_EUI = $"{EndPoint_Identity}/user/get";
            public static readonly string Get_User_From_RoleName = $"{EndPoint_Identity}/user-by-role";
            public static readonly string Put_Update_User = $"{EndPoint_Identity}/update-user";
            public static readonly string Put_Update_User_ClassName = $"{EndPoint_Identity}/add-className";
            public static readonly string Put_Update_User_Admin = $"{EndPoint_Identity}/update-user-admin";
            public static readonly string Put_Update_All_Avater_Users_Default = $"{EndPoint_Identity}/update-avatar-all-develop";
            public static readonly string Delete_User = $"{EndPoint_Identity}/delete-user";
            public static readonly string Put_Change_Password = $"{EndPoint_Identity}/user/change-password";
            public static readonly string Post_Promote_Role = $"{EndPoint_Identity}/promote";
            public static readonly string Post_Refresh_Token = $"{EndPoint_Identity}/token/refreshToken";
            public static readonly string Post_Generate_Token = $"{EndPoint_Identity}/token";
            public static readonly string Delete_Token = $"{EndPoint_Identity}/token";
        }
        public static class PostCodeUrls
        {
            private const string EndPoint_PostCode = "api/postcode";

            public static string Post_SendPostCode => $"{EndPoint_PostCode}/send-postcode";
            public static string Post_CreatePostCode => $"{EndPoint_PostCode}/create-postcode";
            public static string Get_CurrentPostCode => $"{EndPoint_PostCode}/current-postcode";
        }
        public static class TempUrls
        {
            // TempCourseController
            public const string TempCourse_Get = "api/tempCourse";
            public const string TempCourse_UpdateStatus = "api/tempCourse/status";

            // TempRegisterController
            public const string TempRegister_StoreTempUser = "api/tempRegister";
            public const string TempRegister_GetTempUser = "api/tempRegister";

            // TempTimetableController
            public const string TempTimetable_GetTimetable = "api/temptimetable";
            public const string TempTimetable_CreateTimetable = "api/temptimetable";

            public const string TempTimetable_GetClassScheduleVersion = "api/temptimetable/classscheduleversion";
            public const string TempTimetable_CreateClassScheduleVersion = "api/temptimetable/classscheduleversion";

            public const string TempTimetable_CreateCourse = "api/temptimetable/courses";
            public const string TempTimetable_GetCourse = "api/temptimetable/courses";
        }
        public static class OutSourceUrls
        {
            // Base Route
            public const string Base = "api/hubt";

            // Student
            public const string Get_StudentData = $"{Base}/sinhvien";
            public const string Get_Slice_Students = $"{Base}/getSliceStudent";

            public const string Get_StudentList = Base + "/sinhvien/{className}";
            public const string Get_StudentScoreByRoute = Base + "/sinhvien/{masv}/diem";
            public const string Get_StudentScoreByQuery = Base + "/sinhvien/diem";

            // Average Score
            public const string Get_StudentAvgScore = Base + "/diemtb";

            // Timetable
            public const string Get_StudentTimeTable = Base + "/thoikhoabieu";

            // Subject
            public const string Get_Subject = Base + "/monhoc";
        }

    }
}
