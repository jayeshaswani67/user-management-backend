using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using UserApi.Models;

namespace UserApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration config)
        {
            var connectionString = config["MongoDB:ConnectionString"];
            var databaseName = config["MongoDB:DatabaseName"];

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("MongoDB connection string is missing.");

            if (string.IsNullOrWhiteSpace(databaseName))
                throw new Exception("MongoDB database name is missing.");

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            _users = database.GetCollection<User>("users");
        }

        private static string NormalizeRole(string? role)
        {
            var value = role?.Trim();

            return value?.Equals("Employee", StringComparison.OrdinalIgnoreCase) == true
                ? "Employee"
                : "Student";
        }

        private static string NormalizeStatus(string? status)
        {
            var value = status?.Trim();

            return value?.Equals("Inactive", StringComparison.OrdinalIgnoreCase) == true
                ? "Inactive"
                : "Active";
        }

        // Create user
        public async Task CreateAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(
                    nameof(user),
                    "User data is required."
                );
            }

            user.Username = user.Username?.Trim() ?? "";
            user.Email = user.Email?.Trim().ToLowerInvariant() ?? "";
            user.Role = NormalizeRole(user.Role);
            user.Status = NormalizeStatus(user.Status);

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required.");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                throw new ArgumentException(
                    "Password hash is required before creating a user."
                );
            }

            await _users.InsertOneAsync(user);
        }

        // Returns only users for the requested page
        public async Task<List<User>> GetAllAsync(
            int page = 1,
            int pageSize = 10
        )
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, 100);

            return await _users
                .Find(Builders<User>.Filter.Empty)
                .SortByDescending(user => user.Id)
                .Skip((page - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }

        // Count all users, not just one page
        public async Task<long> GetCountAsync()
        {
            return await _users.CountDocumentsAsync(
                Builders<User>.Filter.Empty
            );
        }

        public async Task<User?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is required.", nameof(id));

            return await _users
                .Find(user => user.Id == id.Trim())
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            var escapedEmail = Regex.Escape(email.Trim());

            var filter = Builders<User>.Filter.Regex(
                user => user.Email,
                new BsonRegularExpression($"^{escapedEmail}$", "i")
            );

            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException(
                    "Username is required.",
                    nameof(username)
                );
            }

            var escapedUsername = Regex.Escape(username.Trim());

            var filter = Builders<User>.Filter.Regex(
                user => user.Username,
                new BsonRegularExpression($"^{escapedUsername}$", "i")
            );

            return await _users.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User?> GetByResetTokenAsync(string resetToken)
{
    if (string.IsNullOrWhiteSpace(resetToken))
    {
        return null;
    }

    var filter = Builders<User>.Filter.Eq(
        user => user.ResetToken,
        resetToken.Trim()
    );

    return await _users.Find(filter).FirstOrDefaultAsync();
}

public async Task SaveResetTokenAsync(
    string userId,
    string resetToken,
    DateTime resetTokenExpiry)
{
    if (string.IsNullOrWhiteSpace(userId))
    {
        throw new Exception("User ID is required.");
    }

    var filter = Builders<User>.Filter.Eq(user => user.Id, userId);

    var update = Builders<User>.Update
        .Set(user => user.ResetToken, resetToken)
        .Set(user => user.ResetTokenExpiry, resetTokenExpiry);

    var result = await _users.UpdateOneAsync(filter, update);

    if (result.MatchedCount == 0)
    {
        throw new Exception(
            $"Could not find user with ID '{userId}' while saving reset token."
        );
    }

    if (result.ModifiedCount == 0)
    {
        throw new Exception(
            "MongoDB found the user but did not update the reset token."
        );
    }
}

public async Task UpdatePasswordAndClearResetTokenAsync(
    string userId,
    string passwordHash)
{
    var filter = Builders<User>.Filter.Eq(user => user.Id, userId);

    var update = Builders<User>.Update
        .Set(user => user.PasswordHash, passwordHash)
        .Set(user => user.ResetToken, null)
        .Set(user => user.ResetTokenExpiry, null);

    var result = await _users.UpdateOneAsync(filter, update);

    if (result.MatchedCount == 0)
    {
        throw new Exception("User was not found while resetting password.");
    }
}

       
        public async Task UpdateAsync(string id, User updatedUser)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is required.", nameof(id));

            if (updatedUser == null)
            {
                throw new ArgumentNullException(
                    nameof(updatedUser),
                    "User data is required."
                );
            }

            id = id.Trim();

            var existingUser = await GetByIdAsync(id);

            if (existingUser == null)
                throw new Exception("User not found.");

            existingUser.Username = updatedUser.Username?.Trim() ?? "";
            existingUser.Email = updatedUser.Email?.Trim().ToLowerInvariant() ?? "";
            existingUser.Role = NormalizeRole(updatedUser.Role);
            existingUser.Status = NormalizeStatus(updatedUser.Status);

            // Only change the password if a valid hash was intentionally provided.
            if (!string.IsNullOrWhiteSpace(updatedUser.PasswordHash))
            {
                existingUser.PasswordHash = updatedUser.PasswordHash;
            }

            var result = await _users.ReplaceOneAsync(
                user => user.Id == id,
                existingUser
            );

            if (result.MatchedCount == 0)
                throw new Exception("User not found.");
        }

        public async Task DeleteAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("Id is required.", nameof(id));

            var result = await _users.DeleteOneAsync(
                user => user.Id == id.Trim()
            );

            if (result.DeletedCount == 0)
                throw new Exception("User not found.");
        }

        public async Task<bool> ExistsByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            return await GetByEmailAsync(email) != null;
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return await GetByUsernameAsync(username) != null;
        }
    }
}