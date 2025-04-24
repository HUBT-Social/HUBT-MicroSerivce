using MongoDB.Bson.Serialization.Attributes;

namespace TempRegister_API.Src.Models
{
    public class TempClassScheduleVersion
    {
        [BsonId][BsonElement("ClassName")] public string ClassName { get; set; } = string.Empty;

        [BsonElement("VersionKey")] public string VersionKey { get; set; } = string.Empty;

        [BsonElement("LastUpdate")]
        [BsonDateTimeOptions]
        public DateTime LastUpdate { get; set; }
        [BsonElement("ExpireTime")]
        [BsonDateTimeOptions]
        public DateTime ExpireTime { get; set; }
    }
}
