using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_Identity_Service.Models;

[CollectionName("UserToken")]
public class IdentityToken
{
    [BsonId, BsonElement("userId")] public string UserId { get; set; } = string.Empty;

    [BsonElement("refreshToken")] public string RefreshTo { get; set; } = string.Empty;

    [BsonElement("accessToken")] public string AccessToken { get; set; } = string.Empty;

    [BsonElement("ExpireTime"), BsonDateTimeOptions]
    public DateTime ExpireTime { get; set; }
}