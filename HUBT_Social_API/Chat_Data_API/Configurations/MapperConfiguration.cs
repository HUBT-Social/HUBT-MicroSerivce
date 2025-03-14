using AutoMapper;
using HUBT_Social_Chat_Resources.Dtos.Collections;
using HUBT_Social_Chat_Resources.Models;
using HUBT_Social_Core.Models.DTOs.IdentityDTO;
using HUBT_Social_MongoDb_Service.ASP_Extentions;

namespace Chat_Data_API.Configurations
{
    public class MessageMapper : Profile
    {
        public MessageMapper()
        {
            // Ánh xạ cơ bản: MessageModel -> MessageDTO
            CreateMap<MessageModel, MessageDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.key, opt => opt.MapFrom(src => src.key))
                .ForMember(dest => dest.message, opt => opt.MapFrom(src => src.message))
                .ForMember(dest => dest.createdAt, opt => opt.MapFrom(src => src.createdAt))
                .ForMember(dest => dest.sentBy, opt => opt.MapFrom(src => src.sentBy))
                .ForMember(dest => dest.replyMessage, opt => opt.MapFrom(src => src.replyMessage))
                .ForMember(dest => dest.reactions, opt => opt.MapFrom(src => src.reactions))
                .ForMember(dest => dest.messageType, opt => opt.MapFrom(src => src.messageType))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.status))
                .ForMember(dest => dest.voiceMessageDuration, opt => opt.MapFrom(src => src.voiceMessageDuration))
                .ReverseMap(); // Cho phép ánh xạ ngược từ MessageDTO -> MessageModel

            //// Ánh xạ ReplyMessage -> ReplyMessageDTO
            //CreateMap<ReplyMessage, ReplyMessageDTO>()
            //    .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.id))
            //    .ForMember(dest => dest.message, opt => opt.MapFrom(src => src.message))
            //    .ForMember(dest => dest.sentBy, opt => opt.MapFrom(src => src.sentBy))
            //    .ReverseMap();

            //// Ánh xạ Reaction -> ReactionDTO
            //CreateMap<Reaction, ReactionDTO>()
            //    .ForMember(dest => dest.reactions, opt => opt.MapFrom(src => src.reactions))
            //    .ForMember(dest => dest.reactedUserIds, opt => opt.MapFrom(src => src.reactedUserIds))
            //    .ReverseMap();

            // Ánh xạ danh sách: List<MessageModel> -> List<MessageDTO>
            CreateMap<List<MessageModel>, List<MessageDTO>>()
                .ConvertUsing((source, destination, context) =>
                    source != null
                        ? source.Select(item => context.Mapper.Map<MessageDTO>(item)).ToList()
                        : new List<MessageDTO>());

            // Ánh xạ ngược danh sách: List<MessageDTO> -> List<MessageModel>
            CreateMap<List<MessageDTO>, List<MessageModel>>()
                .ConvertUsing((source, destination, context) =>
                    source != null
                        ? source.Select(item => context.Mapper.Map<MessageModel>(item)).ToList()
                        : new List<MessageModel>());
        }
    }
    public static class MapperConfiguration
    {
        public static IServiceCollection AddMongoMapper(this IServiceCollection services)
        {
            services.AddCustomMappingProfile<MessageMapper>();
            return services;
        }
    }
}
