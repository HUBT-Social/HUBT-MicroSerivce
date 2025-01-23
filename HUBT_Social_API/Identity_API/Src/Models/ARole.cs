using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Identity_API.Src.Models;

[CollectionName("role")]
public class ARole : MongoIdentityRole<Guid>
{
}