using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace Out_Source_Data.Src.Models
{
    [CollectionName("ThoiKhoaBieu")]
    public class ThoiKhoaBieu
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("ClassName")]
        public string ClassName { get; set; } = string.Empty;

        [BsonElement("Day")]
        public string Day { get; set; } = string.Empty;

        [BsonElement("Session")]
        public string Session { get; set; } = string.Empty;
        [BsonElement("Subject")]
        public string Subject { get; set; } = string.Empty;
        [BsonElement("Room")]
        public string Room { get; set; } = string.Empty;
        [BsonElement("ZoomID")]
        public string ZoomID { get; set; } = string.Empty;
    }
}
