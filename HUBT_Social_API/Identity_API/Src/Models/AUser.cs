using AspNetCore.Identity.MongoDbCore.Models;
using HUBT_Social_Core.Settings;
using HUBT_Social_Core.Settings.@enum;
using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace Identity_API.Src.Models;

[CollectionName("user")]
[BsonIgnoreExtraElements]
public class AUser : MongoIdentityUser<Guid>
{
    public string AvataUrl { get; set; } = string.Empty;

    public string? FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; } = string.Empty;

    public Gender Gender { get; set; } = Gender.Other;
    public string FCMToken { get; set; } = string.Empty;
    public string? Status { get; set; } = string.Empty;

    public string DeviceId { get; set; } = string.Empty;


    public DateTime DateOfBirth { get; set; }

    public AUser()
    {
        AvataUrl = KeyStore.GetRandomAvatarDefault(Gender);
    }
}