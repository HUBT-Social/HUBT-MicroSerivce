using Amazon.Runtime.Internal;
using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using Chat_API.Src.Interfaces;
using Hangfire.Mongo.Dto;
using HUBT_Social_Base.ASP_Extentions;
using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Models.DTOs;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_Core.Models.Requests;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Net;

namespace Chat_API.Src.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController(IChatService chatService, IUserService userService,ICourseService courseService) : ControllerBase
    {
        public readonly IChatService _chatService = chatService;
        public readonly IUserService _userService = userService;
        public readonly ICourseService _courseService = courseService;
        private string? ReadTokenFromHeader() => Request.Headers.ExtractBearerToken();


        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest createGroupRequest)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            if (createGroupRequest.UserNames.Count <= 1)
            {
                return BadRequest("Khong du so thanh vien.");
            }
            if (createGroupRequest.GroupType <0 || createGroupRequest.GroupType > 1)
            {
                return BadRequest("Group Type chi nhan hai gia tri 0: P-P, 1: Group");
            }

            ResponseDTO resUserReq = await _userService.GetUserRequest(token);
            if (resUserReq.StatusCode != HttpStatusCode.OK)
            {
                return BadRequest("Không tìm thấy người dùng với token hiện tại.");
            }
            AUserDTO? userReq = resUserReq.ConvertTo<AUserDTO>();
            if (userReq == null) 
            {
                return BadRequest("Loi khi convert thong tin nguoi yeu cau.");
            }

            List<AUserDTO>? userDTOs = await _userService.GetUsersByUserNames(createGroupRequest.UserNames, token);

            if (userDTOs == null || userDTOs.Count == 0)
            {
                return BadRequest("Loi khi lay thong tin chhi tiet cua thanh vien.");
            }
            userDTOs.Add(userReq);

            List<Participant> Pct = userDTOs
                .Select(user => new Participant
                {
                    UserName = user.UserName,
                    Role = user.UserName == userReq.UserName ? ParticipantRole.Owner : ParticipantRole.Member,
                    NickName = user.LastName + " " + user.FirstName,
                    ProfilePhoto = user.AvataUrl
                })
                .ToList();
            CreateGroupRequestData request = new()
            {
                GroupName = createGroupRequest.GroupName,
                Participants = Pct,
                GroupType = createGroupRequest.GroupType == 0 ? TypeChatRoom.SingleChat : TypeChatRoom.GroupChat
            };
            ResponseDTO? response = await _chatService.CreateGroupAsync(request, token);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
                return Ok(new { message = response.Message });


            if (response?.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response.Message);
            }
            return BadRequest(response?.Message);
        }
        
        [HttpPost("create-group-develop")]
        public async Task<IActionResult> CreateGroupDevelop(CreateGroupRequest createGroupRequest)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));
            if (createGroupRequest.UserNames.Count <= 1)
            {
                return BadRequest("Khong du so thanh vien.");
            }

            List<AUserDTO>? userDTOs = await _userService.GetUsersByUserNames(createGroupRequest.UserNames, token);

            if (userDTOs == null || userDTOs.Count == 0)
            {
                return BadRequest("Loi khi lay thong tin chhi tiet cua thanh vien.");
            }
            string owner = userDTOs[0].UserName;

            List<Participant> Pct = userDTOs
                .Select(user => new Participant
                {
                    UserName = user.UserName,
                    Role = user.UserName == owner ? ParticipantRole.Owner : ParticipantRole.Member,
                    NickName = user.LastName + " " + user.FirstName,
                    ProfilePhoto = user.AvataUrl
                })
                .ToList();
            CreateGroupRequestData request = new()
            {
                GroupName = createGroupRequest.GroupName,
                Participants = Pct
            };
            ResponseDTO? response = await _chatService.CreateGroupAsync(request, token);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
                return Ok(new { message = response.Message });


            if (response?.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response.Message);
            }
            return BadRequest(response?.Message);
        }

        [HttpPost("create-group-auto")]
        public async Task<IActionResult> CreateGroupAuto()
        {
            string? token = ReadTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            List<string> groupIds = [];
            int page = 1;
            const int pageSize = 10;

            while (true)
            {
                var courses = await _courseService.GetCourse(page);

                if (courses == null || courses.Count == 0)
                    break;

                foreach (var course in courses)
                {
                    string? groupId = await CreateGroupByCourse(course, token);
                    if (groupId == null)
                        continue;

                    groupIds.Add(groupId);

                    bool updated = await _courseService.PutStatus(course.Id);
                    if (updated)
                    {
                        Console.WriteLine($"Updated tempCourse: {course.Id}");
                    }
                }

                if (courses.Count < pageSize)
                    break;

                page++;
            }

            return Ok(new
            {
                message = $"Created {groupIds.Count} group chat!",
                ids = groupIds
            });
        }

        private async Task<string?> CreateGroupByCourse(CreateGroupByCourse request,string accesstoken)
        {
            string? token = accesstoken;
            if (string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.ClassName) || request.ListUserNames.Count == 0)
            {
                return null;
            }
            if (!string.IsNullOrEmpty(request.Teacher)) 
            {
                request.ListUserNames.Add(request.Teacher);
            }
            
            List<AUserDTO>? userDTOs = await _userService.GetUsersByUserNames(request.ListUserNames, token);
            
            if (userDTOs == null || userDTOs.Count == 0) { return null; }

            AUserDTO? lastUser = userDTOs.LastOrDefault();
            if (lastUser == null) { return null; }
            List<Participant> participants = [];

            foreach (var user in userDTOs)
            {
                ParticipantRole role = ParticipantRole.Member;
                if(user.UserName == lastUser.UserName)
                {
                    role = ParticipantRole.Owner;
                }
                Participant participant = new()
                {
                    UserName = user.UserName,
                    Role = role,
                    NickName = user.LastName + " "+ user.FirstName,
                    ProfilePhoto = user.AvataUrl
                };
                participants.Add(participant);
            }
            CreateGroupRequestData createRequest = new()
            {
                GroupName = request.ClassName+ "_" + request.Subject,
                Participants = participants
            };

            ResponseDTO? response = await _chatService.CreateGroupAsync(createRequest, token);
            if (response != null && response.StatusCode == HttpStatusCode.OK)
            {
                return response.Message;
            }
            return null;

        }



        [HttpGet("get-all-rooms")]
        public async Task<IActionResult> GetAllGroups([FromQuery] int page=1, [FromQuery] int limit=20)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            List<GroupSearchResponse> response = await _chatService.GetAllRoomsAsync(page, limit, token);
            return Ok(response);
        }
        [HttpGet("load-rooms")]
        public async Task<IActionResult> LoadGroupsOfUser([FromQuery] int page =1, [FromQuery] int limit=10)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            List<GroupLoadingResponse> response = await _chatService.GetRoomsOfUserAsync(page, limit, token);
            return Ok(response);
        }

        [HttpGet("search-groups")]
        public async Task<IActionResult> SearchGroup([FromQuery] string keyword, [FromQuery] int page=1, [FromQuery] int limit=5)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            List<GroupSearchResponse> response = await _chatService.SearchGroupsAsync(keyword,page, limit, token);
            return Ok(response);
        }

        [HttpDelete("delete-groups")]
        public async Task<IActionResult> DeleteGroup([FromQuery] string groupId)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            ResponseDTO? response = await _chatService.DeleteGroupAsync(groupId, token);

            if (response != null && response.StatusCode == HttpStatusCode.OK)
                return Ok(new { message = response.Message });


            if (response?.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Unauthorized(response.Message);
            }
            return BadRequest(response?.Message);
        }
    }
}
