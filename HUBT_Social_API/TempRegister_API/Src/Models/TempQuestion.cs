using HUBT_Social_Core.Models.DTOs.ExamDTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TempRegister_API.Src.Models
{
    public class TempQuestion
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string ExamId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public Answer[] Answers { get; set; } = [];
        public int CorrectAnswer { get; set; } = 0;
    }
}
