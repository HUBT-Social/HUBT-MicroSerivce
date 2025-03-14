using HUBT_Social_Chat_Resources.Dtos.Collections.Enum;
using HUBT_Social_Chat_Resources.Dtos.Request.UpdateRequest;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Chat_Service.Interfaces;
using HUBT_Social_MongoDb_Service.ASP_Extentions;
using HUBT_Social_MongoDb_Service.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HUBT_Social_Chat_Service.Services
{
    public class RoomUpdateService(IMongoService<ChatGroupModel> chatGroups, HUBT_Social_Base.Service.ICloudService clouldService) : IRoomUpdateService
    {
        private readonly IMongoService<ChatGroupModel> _chatGroups = chatGroups;
        public readonly HUBT_Social_Base.Service.ICloudService _clouldService = clouldService;

        public async Task<(bool, string)> UpdateGroupNameAsync(string groupId, string newName)
        {
            try
            {
                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(cr => cr.Id == groupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "Chat room does not exist.");
                }

                // Định nghĩa filter (điều kiện cập nhật)
                Expression<Func<ChatGroupModel, bool>> filter = c => c.Id == groupId;

                // Định nghĩa update
                var update = Builders<ChatGroupModel>.Update.Set(c => c.Name, newName);

                // Gọi phương thức UpdateAsync
                bool success = await _chatGroups.UpdateByFilter(filter, update);

                return success ? (true, "Group name updated successfully.") : (false, "No changes made.");
            }
            catch (Exception ex)
            {
                return (false, $"Update failed: {ex.Message}");
            }
        }


        public async Task<(bool, string)> UpdateAvatarGroupAsync(string groupId, IFormFile file)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(groupId))
                {
                    return (false, "The parameter is null.");
                }

                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(c => c.Id == groupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "The group dose not exist.");
                }
                var uploadResult = await _clouldService.UploadFileAsync(file);
                if (uploadResult?.Url == null) 
                {
                    return (false, "Update failed.");
                }

                // Định nghĩa filter (điều kiện cập nhật)
                Expression<Func<ChatGroupModel, bool>> filter = c => c.Id == groupId;

                // Định nghĩa update
                var update = Builders<ChatGroupModel>.Update.Set(c => c.AvatarUrl, uploadResult.Url);

                // Gọi phương thức UpdateAsync
                bool success = await _chatGroups.UpdateByFilter(filter, update);

                if (success)
                {
                    return (true, "Updated.");
                }

                return (false, "Update failed.");
            }
            catch 
            {
                return (false, "Update failed.");
            }
        }


        public async Task<(bool, string)> UpdateNickNameAsync(string groupId, string changedId, string newNickName)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(changedId) || string.IsNullOrEmpty(newNickName))
                {
                    return (false, "The parameter is null.");
                }

                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(c => c.Id == groupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "The group dose not exist.");
                }
                // Tạo filter tìm phòng chat chứa participant có `UserId` cần đổi nickname
                Expression<Func<ChatGroupModel, bool>> filter = r =>
                    r.Id == groupId && r.Participant.Any(p => p.UserId == changedId);

                // Tạo update định nghĩa thay đổi nickname
                var update = Builders<ChatGroupModel>.Update.Set("Participant.$.NickName", newNickName);

                // Gọi phương thức UpdateAsync
                bool success = await _chatGroups.UpdateByFilter(filter, update);

                if (success)
                {
                    return (true, "Updated.");
                }

                return (false, "Update failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateNickNameAsync: {ex.Message}");
                return (false, "Update failed.");
            }
        }

        public async Task<(bool, string)> UpdateParticipantRoleAsync(string groupId, string changedId, ParticipantRole newParticipantRole)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(changedId))
                {
                    return (false, "The parameter is null.");
                }

                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(c => c.Id == groupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "The group dose not exist.");
                }
                // Tạo filter tìm phòng chat chứa participant có `UserId` cần cập nhật
                Expression<Func<ChatGroupModel, bool>> filter = r =>
                    r.Id == groupId && r.Participant.Any(p => p.UserId == changedId);

                // Định nghĩa update thay đổi vai trò của participant
                var update = Builders<ChatGroupModel>.Update.Set("Participant.$.Role", newParticipantRole);

                // Gọi phương thức UpdateAsync
                bool success = await _chatGroups.UpdateByFilter(filter, update);

                if (success)
                {
                    return (true, "Updated.");
                }

                return (false, "Update failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateParticipantRoleAsync: {ex.Message}");
                return (false, "Update failed.");
            }
        }



        public async Task<(bool, string)> JoinRoomAsync(string groupId, Participant? added)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(groupId) || added is null)
                {
                    return (false, "The parameter is null.");
                }

                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(c => c.Id == groupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "The group dose not exist.");
                }
                // Tạo bộ lọc tìm phòng chat dựa trên `GroupId`
                Expression<Func<ChatGroupModel, bool>> filter = c => c.Id == groupId;

                // Cập nhật: Thêm thành viên vào danh sách `Participant` nếu chưa tồn tại
                var update = Builders<ChatGroupModel>.Update.AddToSet(r => r.Participant, added);

                // Thực hiện cập nhật
                var success = await _chatGroups.UpdateByFilter(filter, update);

                if (success)
                {

                    return (true, "Updated.");
                }

                return (false, "Update failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in JoinRoomAsync: {ex.Message}");
                return (false, "Update failed.");
            }
        }


        public async Task<(bool, string)> KickMemberAsync(RemoveMemberRequest request)
        {
            try
            {
                // Kiểm tra đầu vào
                if (request is null || string.IsNullOrEmpty(request.KickedId))
                {
                    return (false, "The parameter is null.");
                }

                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(c => c.Id == request.GroupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "The group dose not exist.");
                }
                // Tạo bộ lọc tìm phòng chat dựa trên `GroupId`
                Expression<Func<ChatGroupModel, bool>> filter = c => c.Id == request.GroupId;

                // Xóa thành viên khỏi danh sách `Participant`
                var update = Builders<ChatGroupModel>.Update.PullFilter(r => r.Participant,
                    p => p.UserId == request.KickedId);

                // Thực hiện cập nhật
                var success = await _chatGroups.UpdateByFilter(filter, update);

                if (success)
                {

                    return (true, "Updated.");
                }

                return (false, "Update failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in KickMemberAsync: {ex.Message}");
                return (false, "Update failed.");
            }
        }

        public async Task<(bool, string)> LeaveRoomAsync(string groupId, string userId)
        {
            try
            {
                // Kiểm tra đầu vào
                if (string.IsNullOrEmpty(groupId) || string.IsNullOrEmpty(userId))
                {
                    return (false, "The parameter is null.");
                }

                // Kiểm tra xem phòng chat có tồn tại không
                var chatRoom = await _chatGroups.Find(c => c.Id == groupId).FirstOrDefaultAsync();
                if (chatRoom == null)
                {
                    return (false, "The group dose not exist.");
                }
                // Tạo bộ lọc tìm phòng chat dựa trên `groupId`
                Expression<Func<ChatGroupModel, bool>> filter = c => c.Id == groupId;

                // Xóa người dùng khỏi danh sách `Participant`
                var update = Builders<ChatGroupModel>.Update.PullFilter(r => r.Participant, p => p.UserId == userId);

                // Thực hiện cập nhật
                var success = await _chatGroups.UpdateByFilter(filter, update);

                if (success)
                {


                    return (true, "Updated.");
                }

                return (false, "Update failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LeaveRoomAsync: {ex.Message}");
                return (false, "Update failed.");
            }
        }

    }
}
