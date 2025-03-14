using Amazon.SecurityToken.Model.Internal.MarshallTransformations;
using Chat_API.Src.Interfaces;
using HUBT_Social_Chat_Resources.Dtos.Request.InitRequest;
using HUBT_Social_Chat_Resources.Dtos.Response;
using HUBT_Social_Core.Decode;
using HUBT_Social_Core.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Chat_API.Src.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController(IChatService chatService) : ControllerBase
    {
        public readonly IChatService _chatService = chatService;

        private string? ReadTokenFromHeader() => Request.Headers.ExtractBearerToken();
        

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroup(CreateGroupRequest createGroupRequest)
        {
            string? token = ReadTokenFromHeader();
            
            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            var response = await _chatService.CreateGroupAsync(createGroupRequest, token);
            return Ok(response);
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
        public async Task<IActionResult> DeleteGroup([FromQuery] string idGroup)
        {
            string? token = ReadTokenFromHeader();

            if (string.IsNullOrEmpty(token))
                return Unauthorized(LocalValue.Get(KeyStore.UnAuthorize));

            string? response = await _chatService.DeleteGroupAsync(idGroup, token);
            return Ok(response);
        }
    }
}
