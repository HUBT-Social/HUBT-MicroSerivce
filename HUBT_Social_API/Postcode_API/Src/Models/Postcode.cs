using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Postcode_API.Src.Models;

public class Postcode
{
    [BsonId, BsonRepresentation(BsonType.ObjectId), BsonElement("ID"),] public string Id { get; set; } = string.Empty;
    [BsonElement("UserAgent")] public string UserAgent { get; set; } = string.Empty;
    [BsonElement("IPAddress")] public string IPAddress { get; set; } = string.Empty;
    [BsonElement("Email")] public string Email { get; set; } = string.Empty;
    [BsonElement("Code")] public string Code { get; set; } = string.Empty;

    [BsonElement("ExpireTime"), BsonDateTimeOptions]
    public DateTime ExpireTime { get; set; }

    public bool ExtendPostcodeExpireTime(int time)
    {
        if (time >= 0)
        {
            ExpireTime = DateTime.Now.AddMinutes(time);
            if (time > 30) ExpireTime = DateTime.Now.AddMinutes(time);
            return true;
        }
        return false;
    }
}