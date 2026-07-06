using MongoDB.Driver;
using UserApi.Models;

namespace UserApi.Services
{
    public class AdminService
    {
        private readonly IMongoCollection<Admin> _admins;

        public AdminService(IConfiguration config)
        {
            var client = new MongoClient(
                config["MongoDB:ConnectionString"]);

            var database = client.GetDatabase(
                config["MongoDB:DatabaseName"]);

            _admins = database.GetCollection<Admin>("admins");
        }

        // POST
        public async Task CreateAsync(Admin admin)
        {
            await _admins.InsertOneAsync(admin);
        }

        // GET ALL
        public async Task<List<Admin>> GetAllAsync()
        {
            return await _admins.Find(_ => true).ToListAsync();
        }

        // GET BY ID
        public async Task<Admin?> GetByIdAsync(string id)
        {
            return await _admins
                .Find(a => a.Id == id)
                .FirstOrDefaultAsync();
        }

        // GET BY EMAIL
        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _admins
                .Find(a => a.Email == email)
                .FirstOrDefaultAsync();
        }

        // GET BY RESET TOKEN
        public async Task<Admin?> GetByResetTokenAsync(string token)
        {
            return await _admins
                .Find(a => a.ResetToken == token)
                .FirstOrDefaultAsync();
        }

        // UPDATE
        public async Task UpdateAsync(string id, Admin admin)
        {
            await _admins.ReplaceOneAsync(
                a => a.Id == id,
                admin);
        }
    }
}