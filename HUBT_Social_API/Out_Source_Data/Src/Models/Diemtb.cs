using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using MongoDbGenericRepository.Attributes;
namespace Out_Source_Data.Src.Models

{
    [CollectionName("Diemtb")]
    public class Diemtb
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("MaSV")]
        public string MaSV { get; set; } = string.Empty;

        [BsonElement("Diem_TB_10")]
        public double DiemTB10 { get; set; }

        [BsonElement("Diem_TB_4")]
        public double DiemTB4 { get; set; }
        
    }
}
