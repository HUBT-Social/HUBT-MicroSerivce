using HUBT_Social_Core.Models.OutSourceDataDTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TempRegister_API.Src.Models
{
    public class TempCourse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("CourseID")]
        public string CourseID { get; set; } = string.Empty;

        [BsonElement("StudentIDs")]
        public List<string> StudentIDs { get; set; } = new List<string>();

        [BsonElement("TimeTableDTO")]
        public TimeTable TimeTableDTO { get; set; } = new TimeTable();
    }
}
