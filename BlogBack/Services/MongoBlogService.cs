using BlogBack.Models;
using MongoDB.Driver;

namespace BlogBack.Services
{
    public class MongoBlogService
    {
        private readonly IMongoCollection<MongoBlog> _blogs;

        public MongoBlogService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _blogs = database.GetCollection<MongoBlog>(config["MongoDB:BlogCollection"]);
        }

        public async Task StoreBlogAsync(MongoBlog blog)
        {
            blog.CreatedAt = DateTime.UtcNow;
            await _blogs.InsertOneAsync(blog);
        }
        public async Task DeleteBlogAsync(Guid supabaseId)
        {
            var filter = Builders<MongoBlog>.Filter.Eq(b => b.BlogId, supabaseId.ToString());
            await _blogs.DeleteOneAsync(filter);
        }


        public async Task UpdateBlogAsync(MongoBlog blog)
        {
            var filter = Builders<MongoBlog>.Filter.Eq(b => b.BlogId, blog.BlogId);
            var update = Builders<MongoBlog>.Update
                .Set(b => b.Title, blog.Title)
                .Set(b => b.Description, blog.Description)
                .Set(b => b.Content, blog.Content)
                .Set(b => b.Author, blog.Author)
                .Set(b => b.AuthorEmail, blog.AuthorEmail)
                .Set(b => b.CreatedAt, blog.CreatedAt);

            await _blogs.UpdateOneAsync(filter, update);
        }



    }
}
