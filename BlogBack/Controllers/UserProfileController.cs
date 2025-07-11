using BlogBack.Models;
using BlogBack.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Supabase;
using System.Linq;
using System.Threading.Tasks;

namespace BlogBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly Supabase.Client _client;
        private readonly MongoUserProfileService _mongoUserProfileService;
        public UserProfileController(IOptions<SupabaseConfig> config , MongoUserProfileService mongoUserProfileService)
        {
            var options = new SupabaseOptions { AutoConnectRealtime = false };
            _client = new Supabase.Client(config.Value.Url, config.Value.ApiKey, options);
            Task.Run(() => _client.InitializeAsync()).Wait();

            _mongoUserProfileService = mongoUserProfileService;
        }

        [HttpPost("save")]
        public async Task<IActionResult> SaveProfile([FromBody] UserProfileDto dto)
        {
            var result = await _client
                .From<UserProfile>()
                .Where(x => x.Email == dto.Email)
                .Get();

            var existing = result.Models.FirstOrDefault();
            var createdAt = DateTime.UtcNow;

            if (existing != null)
            {
                existing.Name = dto.Name;
                existing.Age = dto.Age;
                existing.Bio = dto.Bio;
                existing.CreatedAt = createdAt;

                await _client.From<UserProfile>().Update(existing);
            }
            else
            {
                var newProfile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    Email = dto.Email,
                    Name = dto.Name,
                    Age = dto.Age,
                    Bio = dto.Bio,
                    CreatedAt = createdAt
                };

                await _client.From<UserProfile>().Insert(newProfile);
            }

            // ✅ MongoDB Save
            var mongoProfile = new MongoUserProfile
            {
                Email = dto.Email,
                Name = dto.Name,
                Age = dto.Age,
                Bio = dto.Bio,
                CreatedAt = createdAt
            };

            await _mongoUserProfileService.StoreUserProfileAsync(mongoProfile);

            return Ok(existing != null ? "Profile updated." : "Profile created.");
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetProfile(string email)
        {
            var result = await _client
                .From<UserProfile>()
                .Where(x => x.Email == email)
                .Get();

            var profile = result.Models.FirstOrDefault();
            if (profile == null) return NotFound();

            var dto = new UserProfileDto
            {
                Id = profile.Id,
                Email = profile.Email,
                Name = profile.Name,
                Age = profile.Age,
                Bio = profile.Bio,
                CreatedAt = profile.CreatedAt
            };

            return Ok(dto);
        }
    }
}
