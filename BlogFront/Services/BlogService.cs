using BlogFront.Models;
using BlogFront.Pages;
using BlogFront.Models;

using System.Net.Http.Json;

namespace BlogFront.Services
{
    public class BlogService
    {
        private readonly HttpClient _http;

        public BlogService(HttpClient http)
        {
            _http = http;
        }

        public async Task<string?> UploadBlogAsync(BlogPost blog)
        {
            var response = await _http.PostAsJsonAsync("api/blog/upload", blog);

            if (response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }


        public async Task<List<BlogPost>> GetBlogsAsync()
        {
            var res = await _http.GetFromJsonAsync<List<BlogPost>>("api/blog/all");

            return res ?? new List<BlogPost>();
        }

        public async Task<BlogPost?> GetBlogByIdAsync(Guid id)
        {
            return await _http.GetFromJsonAsync<BlogPost>($"api/blog/getbyid/{id}");
        }
        public async Task<List<BlogPost>> GetBlogsByUserAsync(string email)
        {
            var response = await _http.GetFromJsonAsync<List<BlogPost>>($"api/blog/byuser/{email}");
            return response ?? new List<BlogPost>();
        }
        public async Task DeleteBlogAsync(Guid id)
        {
            await _http.DeleteAsync($"api/blog/delete/{id}");
        }
        public async Task<string?> UpdateBlogAsync(BlogPost blog)
        {
            var response = await _http.PutAsJsonAsync($"api/blog/update/{blog.Id}", blog);
            if (response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadAsStringAsync();
        }

    }
}
