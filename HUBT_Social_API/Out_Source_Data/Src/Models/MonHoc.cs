using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Out_Source_Data.Src.Models
{
    public class MonHoc
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("MaNganh")]
        public string MaNganh { get; set; } = string.Empty;

        [BsonElement("Khoas")]
        public double Khoas { get; set; }

        [BsonElement("TenMon")]
        public string TenMon { get; set; } = string.Empty;

        [BsonElement("SoTin")]
        public double SoTin { get; set; }
    }
}
