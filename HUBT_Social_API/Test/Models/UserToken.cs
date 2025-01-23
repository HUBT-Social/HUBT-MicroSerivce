using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Test.Models;

[CollectionName("RefreshToken")]
public class UserToken
{
    [BsonId, BsonElement("userId")] public string UserId { get; set; } = string.Empty;

    [BsonElement("refreshToken")] public string RefreshTo { get; set; } = string.Empty;

    [BsonElement("accessToken")] public string AccessToken { get; set; } = string.Empty;

    [BsonElement("ExpireTime"), BsonDateTimeOptions]
    public DateTime ExpireTime { get; set; }
}