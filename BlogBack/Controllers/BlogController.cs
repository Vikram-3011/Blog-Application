using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Supabase;
using BlogBack.Models;
using static Supabase.Postgrest.Constants;
using BlogBack.Services;


[Route("api/[controller]")]
[ApiController]
public class BlogController : ControllerBase
{
    private readonly Supabase.Client _client;
    private readonly MongoBlogService _mongoService;

    public BlogController(IOptions<SupabaseConfig> config, MongoBlogService mongoService)
    {
        var options = new SupabaseOptions { AutoConnectRealtime = false };

        if (string.IsNullOrEmpty(config.Value.Url) || string.IsNullOrEmpty(config.Value.ApiKey))
        {
            throw new ArgumentNullException("Supabase URL or API Key is missing from configuration.");
        }

        _client = new Supabase.Client(config.Value.Url, config.Value.ApiKey, options);
        _client.InitializeAsync().Wait();

        _mongoService = mongoService;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadBlog([FromBody] BlogPost blog)
    {
        // 1. Store in Supabase

        blog.Tags = blog.Tags?.Select(t => t.ToLower()).ToList();

        await _client.From<BlogPost>().Insert(blog);

    // 2. Store same blog in MongoDB
    var mongoBlog = new MongoBlog
    {
        BlogId = blog.Id.ToString(),
        Title = blog.Title,
        Content = blog.Content,
        Description = blog.Description,
        Author = blog.Author,
        AuthorEmail = blog.AuthorEmail,
        CreatedAt = blog.CreatedAt,
        Tags = blog.Tags // ✅ Add this line
    };

    await _mongoService.StoreBlogAsync(mongoBlog);

    return Ok("Blog uploaded to both Supabase and MongoDB.");
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllBlogs()
    {
        var result = await _client.From<BlogPost>().Get();

        var blogs = result.Models.Select(b => new BlogPostDto
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            Author = b.Author,
            AuthorEmail = b.AuthorEmail,
            CreatedAt = b.CreatedAt,
            Description = b.Description,
            Tags = b.Tags // ✅ Add this line
        }).ToList();


        return Ok(blogs);
    }

    [HttpGet("getbyid/{id}")]
    public async Task<IActionResult> GetBlogById(Guid id)
    {
        var result = await _client
            .From<BlogPost>()
            .Filter("id", Operator.Equals, id.ToString())
            .Get();

        var blog = result.Models.FirstOrDefault();
        if (blog == null)
            return NotFound("Blog not found");

        var dto = new BlogPostDto
        {
            Title = blog.Title,
            Content = blog.Content,
            Author = blog.Author,
            AuthorEmail = blog.AuthorEmail,
            CreatedAt = blog.CreatedAt,
            Description = blog.Description,
            Tags = blog.Tags // ✅ Add this line
        };


        return Ok(dto);
    }

    [HttpGet("byuser/{email}")]
    public async Task<IActionResult> GetBlogsByUser(string email)
    {
        var result = await _client
            .From<BlogPost>()
            .Filter("author_email", Operator.Equals, email)
            .Get();

        var blogs = result.Models.Select(b => new BlogPostDto
        {
            Id = b.Id,
            Title = b.Title,
            Description = b.Description,
            Content = b.Content,
            Author = b.Author,
            AuthorEmail = b.AuthorEmail,
            CreatedAt = b.CreatedAt,
            Tags = b.Tags // ✅ Add this line
        }).ToList();


        return Ok(blogs);
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteBlog(Guid id, [FromQuery] string requesterEmail)
    {
        await _client.InitializeAsync();

        // Check if user is admin
        var isAdmin = await _client
            .From<Admin>()
            .Filter("email", Operator.Equals, requesterEmail.ToLower())
            .Get();

        var isUserAdmin = isAdmin.Models.Any();

        // Fetch the blog to verify ownership or allow admin
        var result = await _client
            .From<BlogPost>()
            .Filter("id", Operator.Equals, id.ToString())
            .Get();

        var blog = result.Models.FirstOrDefault();

        if (blog == null)
            return NotFound("Blog not found.");

        if (!isUserAdmin && blog.AuthorEmail.ToLower() != requesterEmail.ToLower())
            return Unauthorized("You are not allowed to delete this blog.");

        // Delete from Supabase
        await _client.From<BlogPost>().Where(b => b.Id == id).Delete();

        await _mongoService.DeleteBlogAsync(id);

        return Ok("Blog deleted successfully.");
    }



    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateBlog(Guid id, [FromBody] BlogPost updatedBlog, [FromQuery] string requesterEmail)
    {

        await _client.InitializeAsync();

        var isAdmin = await _client
            .From<Admin>()
            .Filter("email", Operator.Equals, requesterEmail.ToLower())
            .Get();

        var isUserAdmin = isAdmin.Models.Any();

        var existing = await _client
            .From<BlogPost>()
            .Filter("id", Operator.Equals, id.ToString())
            .Get();

        var blog = existing.Models.FirstOrDefault();
        if (blog == null)
            return NotFound("Blog not found");


        if (!isUserAdmin && blog.AuthorEmail.ToLower() != requesterEmail.ToLower())
            return Unauthorized("You are not allowed to update this blog.");

        // Update fields
        blog.Title = updatedBlog.Title;
        blog.Description = updatedBlog.Description;
        blog.Content = updatedBlog.Content;

        await _client.From<BlogPost>().Update(blog);

        // 2. Update in MongoDB
        var mongoBlog = new MongoBlog
        {
            BlogId = id.ToString(), // Supabase ID for matching
            Title = updatedBlog.Title,
            Description = updatedBlog.Description,
            Content = updatedBlog.Content,
            Author = blog.Author,
            AuthorEmail = blog.AuthorEmail,
            CreatedAt = blog.CreatedAt
            // Don't set Id
        };


        await _mongoService.UpdateBlogAsync(mongoBlog);

        return Ok("Blog updated successfully.");
    }
    [HttpGet("bytag")]
    public async Task<IActionResult> GetBlogsByTag([FromQuery] string tag)
    {
        if (string.IsNullOrWhiteSpace(tag) || tag.ToLower() == "all")
        {
            return await GetAllBlogs();
        }


        var lowerTag = tag.ToLower();

        // 🔥 Wrap tag as array to use "contains" operator correctly
        var result = await _client
            .From<BlogPost>()
            .Filter("tags", Operator.Contains, new List<string> { lowerTag })
            .Get();

        var blogs = result.Models.Select(b => new BlogPostDto
        {
            Id = b.Id,
            Title = b.Title,
            Content = b.Content,
            Author = b.Author,
            AuthorEmail = b.AuthorEmail,
            CreatedAt = b.CreatedAt,
            Description = b.Description,
            Tags = b.Tags
        }).ToList();

        return Ok(blogs);
    }



}


