using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Out_Source_Data.Src.Models
{
    public class DiemSinhVien
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("MaSV")]
        public string Masv { get; set; } = string.Empty;
        [BsonElement("TenMonHoc")]
        public string TenMonHoc { get; set; } = string.Empty;
        [BsonElement("Diem")]
        public double Diem { get; set; }
    }
}
