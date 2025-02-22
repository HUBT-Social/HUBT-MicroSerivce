using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;

namespace Out_Source_Data.Src.Models
{
    [CollectionName("SinhVien")]

    public class SinhVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("MASV")]
        public string Masv { get; set; } = string.Empty;

        [BsonElement("HoTen")]
        public string HoTen { get; set; } = string.Empty;

        [BsonElement("NgaySinh")]
        public string NgaySinh { get; set; } = string.Empty;
        [BsonElement("GioiTinh")]
        public string GioiTinh { get; set; } = string.Empty;

        [BsonElement("TenLop")]
        public string TenLop { get; set; } = string.Empty;
    }
}
