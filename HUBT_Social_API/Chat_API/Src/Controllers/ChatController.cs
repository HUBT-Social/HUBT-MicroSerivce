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
            List<AUserDTO> userDTOs = new List<AUserDTO>();
            ResponseDTO resAllUser = await _userService.GetAllUser(token);
            Console.WriteLine(resAllUser.ToJson());
            ResponseDTO resUserReq = await _userService.GetUserRequest(token);
            List<AUserDTO>? allUser =  resAllUser.ConvertTo<List<AUserDTO>>();
            AUserDTO? userReq =  resUserReq.ConvertTo<AUserDTO>();

            if (allUser == null || !allUser.Any() || userReq == null)
            {
                return BadRequest(new { message = "Param is null" });
            }
            userDTOs.AddRange(allUser);
            userDTOs.Add(userReq);
            var filteredUsers = userDTOs
                    .Where(user => createGroupRequest.UserNames.Contains(user.UserName) || user.UserName == userReq.UserName)
                    .ToList();

            List<Participant> Pct = filteredUsers
                .Select(user => new Participant
                {
                    UserName = user.Id.ToString(),
                    Role = user.Id.ToString() == userReq.Id.ToString() ? ParticipantRole.Owner : ParticipantRole.Member,
                    NickName = user.UserName, // Hoặc một giá trị mặc định
                    ProfilePhoto = user.AvataUrl
                })
                .ToList();
            CreateGroupRequestData request = new CreateGroupRequestData
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
            List<string> ids = new List<string>();
            int times = 1;
            while (true) 
            {
                List<CreateGroupByCourse>? res = await _courseService.GetCourse(times);
                if (res != null && res.Count>0) {
                    foreach (CreateGroupByCourse course in res)
                    {
                        string? idGroup = await CreateGroupByCourse(course, token);
                        if (idGroup == null) continue;
                        ids.Add(idGroup);
                    }
                }
                 if (res != null && res.Count < 10) { break; }
                times++;
            }
            return Ok(new { message = $"Created {ids.Count} group chat!", ids = ids.ToArray() });
        }
        private async Task<string?> CreateGroupByCourse(CreateGroupByCourse request,string accesstoken)
        {
            string? token = accesstoken;
            if (string.IsNullOrEmpty(request.Subject) || string.IsNullOrEmpty(request.ClassName) || !request.listUserNames.userNames.Any())
            {
                return null;
            }
            if (!string.IsNullOrEmpty(request.Teacher)) 
            {
                request.listUserNames.userNames.Add(request.Teacher);
            }
            
            List<AUserDTO>? userDTOs = await _userService.GetUsersByUserNames(request.listUserNames, token);
            
            if (userDTOs == null || !userDTOs.Any()) { return null; }

            AUserDTO? lastUser = userDTOs.LastOrDefault();
            if (lastUser == null) { return null; }
            List<Participant> participants = new List<Participant>();

            foreach (var user in userDTOs)
            {
                ParticipantRole role = ParticipantRole.Member;
                if(user.UserName == lastUser.UserName)
                {
                    role = ParticipantRole.Owner;
                }
                Participant participant = new Participant
                {
                    UserName = user.UserName,
                    Role = role,
                    NickName = user.LastName + " "+ user.FirstName,
                    ProfilePhoto = user.AvataUrl
                };
                participants.Add(participant);
            }
            CreateGroupRequestData createRequest = new CreateGroupRequestData
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

            List<GroupLoadingResponse> response = await _chatService.GetRoomsOfUserIdAsync(page, limit, token);
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
