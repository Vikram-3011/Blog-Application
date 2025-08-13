using BlogFront.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlogFront.Services
{
    public class BlogService
    {
        private readonly HttpClient _http;

        // Simple in-memory cache
        private readonly Dictionary<string, object> _cache = new();
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5);
        private readonly Dictionary<string, DateTime> _cacheTimestamps = new();

        public BlogService(HttpClient http)
        {
            _http = http;
        }

        private bool IsCacheValid(string key)
        {
            return _cache.ContainsKey(key) &&
                   _cacheTimestamps.ContainsKey(key) &&
                   (DateTime.Now - _cacheTimestamps[key]) < _cacheDuration;
        }

        private void SetCache(string key, object data)
        {
            _cache[key] = data;
            _cacheTimestamps[key] = DateTime.Now;
        }

        public void ClearCache()
        {
            _cache.Clear();
            _cacheTimestamps.Clear();
        }

        public async Task<string?> UploadBlogAsync(BlogPost blog)
        {
            var response = await _http.PostAsJsonAsync("api/blog/upload", blog);

            if (response.IsSuccessStatusCode)
            {
                ClearCache(); // New blog, clear cache
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<BlogPost>> GetBlogsAsync()
        {
            string cacheKey = "GetBlogsAsync";

            if (IsCacheValid(cacheKey))
                return (List<BlogPost>)_cache[cacheKey];

            var res = await _http.GetFromJsonAsync<List<BlogPost>>("api/blog/all") ?? new List<BlogPost>();

            SetCache(cacheKey, res);
            return res;
        }

        public async Task<BlogPost?> GetBlogByIdAsync(Guid id)
        {
            string cacheKey = $"GetBlogByIdAsync_{id}";

            if (IsCacheValid(cacheKey))
                return (BlogPost?)_cache[cacheKey];

            var blog = await _http.GetFromJsonAsync<BlogPost>($"api/blog/getbyid/{id}");

            if (blog != null)
                SetCache(cacheKey, blog);

            return blog;
        }

        public async Task<List<BlogPost>> GetBlogsByUserAsync(string email)
        {
            string cacheKey = $"GetBlogsByUserAsync_{email}";

            if (IsCacheValid(cacheKey))
                return (List<BlogPost>)_cache[cacheKey];

            var response = await _http.GetFromJsonAsync<List<BlogPost>>($"api/blog/byuser/{email}") ?? new List<BlogPost>();

            SetCache(cacheKey, response);
            return response;
        }

        public async Task<bool> DeleteBlogAsync(Guid id, string requesterEmail)
        {
            var response = await _http.DeleteAsync($"api/blog/delete/{id}?requesterEmail={Uri.EscapeDataString(requesterEmail)}");
            if (response.IsSuccessStatusCode)
                ClearCache(); // Deleted blog, clear cache

            return response.IsSuccessStatusCode;
        }

        public async Task<string?> UpdateBlogAsync(BlogPost blog, string requesterEmail)
        {
            var response = await _http.PutAsJsonAsync(
                $"api/blog/update/{blog.Id}?requesterEmail={Uri.EscapeDataString(requesterEmail)}", blog);

            if (response.IsSuccessStatusCode)
            {
                ClearCache(); // Updated blog, clear cache
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<List<Blog>> GetAllBlogsAsync()
        {
            string cacheKey = "GetAllBlogsAsync";

            if (IsCacheValid(cacheKey))
                return (List<Blog>)_cache[cacheKey];

            var response = await _http.GetAsync("api/blog");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var blogs = JsonSerializer.Deserialize<List<Blog>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Blog>();

                SetCache(cacheKey, blogs);
                return blogs;
            }

            return new List<Blog>();
        }

        public async Task<List<Blog>> GetBlogsByTagAsync(string tag)
        {
            string cacheKey = $"GetBlogsByTagAsync_{tag}";

            if (IsCacheValid(cacheKey))
                return (List<Blog>)_cache[cacheKey];

            string url = string.IsNullOrEmpty(tag) || tag.ToLower() == "all"
                ? "api/blog/all"
                : $"api/blog/bytag?tag={Uri.EscapeDataString(tag)}";

            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var blogs = JsonSerializer.Deserialize<List<Blog>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? new List<Blog>();

                SetCache(cacheKey, blogs);
                return blogs;
            }

            return new List<Blog>();
        }
    }
}
