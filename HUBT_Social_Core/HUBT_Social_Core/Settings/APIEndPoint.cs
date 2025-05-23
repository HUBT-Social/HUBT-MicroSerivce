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
            private const string EndPointChatDataChat = "api/chat";
            private const string EndPointChatDataRoom = "api/room";

            public static readonly string PostCreateGroup = $"{EndPointChatDataChat}/create-group";
            public static readonly string DeleteGroup = $"{EndPointChatDataChat}/delete-group";
            public static readonly string GetSearchGroup = $"{EndPointChatDataChat}/search-groups";
            public static readonly string GetAllGroup = $"{EndPointChatDataChat}/get-all-rooms";
            public static readonly string GetUserGroup = $"{EndPointChatDataChat}/get-rooms-of-user";
            public static readonly string PutUpdateGroupName = $"{EndPointChatDataRoom}/update-group-name";
            public static readonly string PutUpdateGroupAvatar = $"{EndPointChatDataRoom}/update-avatar";
            public static readonly string PutUpdateNickName = $"{EndPointChatDataRoom}/update-nickname";
            public static readonly string PutUpdateParticipantRole = $"{EndPointChatDataRoom}/update-participant-role";
            public static readonly string PostJoinGroup = $"{EndPointChatDataRoom}/join-room";
            public static readonly string PostKickMember = $"{EndPointChatDataRoom}/kick-member";
            public static readonly string PostLeaveGroup = $"{EndPointChatDataRoom}/leave-room";
            public static readonly string GetHistoryContent = $"{EndPointChatDataRoom}/message-history";
            public static readonly string GetMembers = $"{EndPointChatDataRoom}/get-members";
        }

        public static class IdentityUrls
        {
            private const string EndPointIdentity = "api/identity";

            public static readonly string PostLogin = $"{EndPointIdentity}/auth/vertifile-account";
            public static readonly string PostSignUp = $"{EndPointIdentity}/auth/create-account";
            public static readonly string GetAllUser = EndPointIdentity;
            public static readonly string GetUserInListUserName = $"{EndPointIdentity}/users-in-list-userName";
            public static readonly string GetCurrentUser = $"{EndPointIdentity}/user";
            public static readonly string GetUserFromEUI = $"{EndPointIdentity}/user/get";
            public static readonly string GetUserFromRoleName = $"{EndPointIdentity}/user-by-role";
            public static readonly string PutUpdateUser = $"{EndPointIdentity}/update-user";
            public static readonly string PutUpdateUserClassName = $"{EndPointIdentity}/add-className";
            public static readonly string PutUpdateUserAdmin = $"{EndPointIdentity}/update-user-admin";
            public static readonly string PutUpdateAllAvaterUsersDefault = $"{EndPointIdentity}/update-avatar-all-develop";
            public static readonly string DeleteUser = $"{EndPointIdentity}/delete-user";
            public static readonly string PutChangePassword = $"{EndPointIdentity}/user/change-password";
            public static readonly string PostPromoteRole = $"{EndPointIdentity}/promote";
            public static readonly string PostRefreshToken = $"{EndPointIdentity}/token/refreshToken";
            public static readonly string PostGenerateToken = $"{EndPointIdentity}/token";
            public static readonly string DeleteToken = $"{EndPointIdentity}/token";
        }
        public static class PostCodeUrls
        {
            private const string EndPointPostCode = "api/postcode";

            public static string PostSendPostCode => $"{EndPointPostCode}/send-postcode";
            public static string PostCreatePostCode => $"{EndPointPostCode}/create-postcode";
            public static string GetCurrentPostCode => $"{EndPointPostCode}/current-postcode";
        }
        public static class TempUrls
        {
            // TempCourseController
            public const string TempCourseGet = "api/tempCourse";
            public const string TempCourseUpdateStatus = "api/tempCourse/status";

            // TempRegisterController
            public const string TempRegisterStoreTempUser = "api/tempRegister";
            public const string TempRegisterGetTempUser = "api/tempRegister";

            // TempTimetableController
            public const string TempTimetableGetTimetable = "api/temptimetable";
            public const string TempTimetableCreateTimetable = "api/temptimetable";

            public const string TempTimetableGetClassScheduleVersion = "api/temptimetable/classscheduleversion";
            public const string TempTimetableCreateClassScheduleVersion = "api/temptimetable/classscheduleversion";

            public const string TempTimetableCreateCourse = "api/temptimetable/courses";
            public const string TempTimetableGetCourse = "api/temptimetable/courses";

            // TempExamController
            public const string TempExam = "api/tempexam";
            public const string TempExamMajor = "api/tempexam/major";
        }
        public static class OutSourceUrls
        {
            // Base Route
            public const string Base = "api/hubt";

            // Student
            public const string GetStudentData = $"{Base}/sinhvien";
            public const string GetSliceStudents = $"{Base}/getSliceStudent";

            public const string GetStudentList = Base + "/sinhvien/{className}";
            public const string GetStudentScoreByRoute = Base + "/sinhvien/{masv}/diem";
            public const string GetStudentScoreByQuery = Base + "/sinhvien/diem";

            // Average Score
            public const string GetStudentAvgScore = Base + "/diemtb";

            // Timetable
            public const string GetStudentTimeTable = Base + "/thoikhoabieu";

            // Subject
            public const string GetSubject = Base + "/monhoc";
        }

    }
}
