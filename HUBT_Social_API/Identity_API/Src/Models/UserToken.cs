using HUBT_Social_Identity_Service.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Identity_API.Src.Models;

[CollectionName("UserToken")]
public class UserToken : IdentityToken
{

}