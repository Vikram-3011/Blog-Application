using System.Net.Http.Json;
using BlogFront.Models;

namespace BlogFront.Services
{
    public class ProfileService
    {
        private readonly HttpClient _http;

        public ProfileService(HttpClient http)
        {
            _http = http;
        }

        public async Task<UserProfile?> GetProfileAsync(string email)
        {
            var encoded = Uri.EscapeDataString(email);
            var response = await _http.GetAsync($"api/userprofile/{encoded}");

            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<UserProfile>();
        }

        public async Task<string> SaveProfileAsync(UserProfile profile)
        {
            var res = await _http.PostAsJsonAsync("api/userprofile/save", profile);
            return await res.Content.ReadAsStringAsync();
        }
    }
}
