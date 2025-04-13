using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Out_Source_Data.Src.Models
{

    public class HocPhan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("Manganh")]
        public string Manganh { get; set; } = string.Empty;

        [BsonElement("Khoa")]
        public double Khoa { get; set; }

        [BsonElement("Tenmon")]
        public string Tenmon { get; set; } = string.Empty;

        [BsonElement("Sotinchi")]
        public double Sotinchi { get; set; }
    }
}
