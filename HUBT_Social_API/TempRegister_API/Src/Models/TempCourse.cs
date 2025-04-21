using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using HUBT_Social_Core.Models.OutSourceDataDTO;

namespace TempRegister_API.Src.Models
{
    public class TempCourse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]

        public string Id { get; set; } = string.Empty;
        public string CourseID { get; set; } = string.Empty;
        public string[] StudentIDs { get; set; } = [];
        public TimeTableDTO TimeTableDTO { get; set; } = new();
    }
}
