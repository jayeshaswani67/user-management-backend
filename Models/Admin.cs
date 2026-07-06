using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UserApi.Models
{
    public class Admin
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("username")]
        public required string Username { get; set; }
        
        [BsonElement("email")]
        public required string Email { get; set; }
        
        [BsonElement("passwordHash")]
        public required string PasswordHash { get; set; } // Use PasswordHash
        
        // JsonIgnore is hidding the token from swagger UI 
        [BsonIgnore]
        [JsonIgnore]
        public string? JwtToken { get; set; }
        
        // Reset Password Token
        [BsonElement("resetToken")]
        [JsonIgnore]
        public string? ResetToken { get; set; }
        
        [BsonElement("resetTokenExpiry")]
        [JsonIgnore]
        public DateTime? ResetTokenExpiry { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    }
}