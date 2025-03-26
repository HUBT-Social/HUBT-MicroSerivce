using HUBT_Social_Core.Settings.@enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TempRegister_API.Src.Models
{
    public class TempTimetable
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("ClassName")] public string ClassName { get; set; } = string.Empty;
        [BsonElement("StartTime")] public DateTime StartTime { get; set; } 
        [BsonElement("EndTime")] public DateTime? EndTime { get; set; }
        [BsonElement("Subject")] public string Subject { get; set; } = string.Empty;
        [BsonElement("Room")] public string Room { get; set; } = string.Empty;
        [BsonElement("ZoomID")] public string? ZoomID { get; set; } = string.Empty;
        [BsonElement("Type")] public TimeTableType Type { get; set; } = TimeTableType.Study;
    }
}
