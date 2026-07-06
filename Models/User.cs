using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserApi.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("username")]
        public string Username { get; set; } = "";
        
        [BsonElement("email")]
        public string Email { get; set; } = "";

        public string Role { get; set; } = "Student";
        
        public string Status { get; set; } = "Active";
        
         [BsonElement("passwordHash")]
         [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("Token")]
        [JsonIgnore]
        public string? Token { get; set; }
        
        // Reset token - stored in MongoDB but hidden from API responses
        [BsonElement("resetToken")]
        [JsonIgnore]
        public string? ResetToken { get; set; }
        
        // Reset token expiry - stored in MongoDB but hidden from API responses
        [BsonElement("resetTokenExpiry")]
        [JsonIgnore]
        public DateTime? ResetTokenExpiry { get; set; }
    }
}