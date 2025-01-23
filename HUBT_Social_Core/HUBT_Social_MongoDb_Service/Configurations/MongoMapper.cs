
using AutoMapper;
using HUBT_Social_Core.Models.DTOs;

namespace HUBT_Social_MongoDb_Service.Configurations
{
    public class MongoMapper<TCollection, TCollectionDTO> : Profile
        where TCollection : class, new()
        where TCollectionDTO : class, new()
    {
        public MongoMapper()
        {
            CreateMap<TCollection,TCollectionDTO>().ReverseMap();
        }
    }
}
