using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Test.Models;

[CollectionName("user")]
[BsonIgnoreExtraElements]
public class AUser : MongoIdentityUser<Guid>
{
    public string AvataUrl { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

}