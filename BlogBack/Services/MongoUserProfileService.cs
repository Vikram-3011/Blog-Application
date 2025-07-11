using BlogBack.Models;
using MongoDB.Driver;

namespace BlogBack.Services
{
    public class MongoUserProfileService
    {
        private readonly IMongoCollection<MongoUserProfile> _profiles;

        public MongoUserProfileService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _profiles = database.GetCollection<MongoUserProfile>("UserProfiles");
        }

        public async Task StoreUserProfileAsync(MongoUserProfile profile)
        {
            var filter = Builders<MongoUserProfile>.Filter.Eq(p => p.Email, profile.Email);
            var existing = await _profiles.Find(filter).FirstOrDefaultAsync();

            if (existing != null)
            {
                // Update existing
                profile.Id = existing.Id;
                await _profiles.ReplaceOneAsync(filter, profile);
            }
            else
            {
                await _profiles.InsertOneAsync(profile);
            }
        }
    }
}
