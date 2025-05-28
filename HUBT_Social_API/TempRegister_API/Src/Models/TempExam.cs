using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HUBT_Social_Core.Models.DTOs.ExamDTO;

namespace TempRegister_API.Src.Models
{
    public class TempExam
    {
        [BsonId,BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; } = 0;
        public string Image { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int Credits { get; set; } = 0;
        public int QuestionCount { get; set; } = 0;

        public Question[] Questions { get; set; } = [];
    }

}
