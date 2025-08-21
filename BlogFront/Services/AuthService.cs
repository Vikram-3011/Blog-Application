using System.Net.Http.Json;
using MudBlazor;
using BlogFront.Models;
using BlogUser = BlogFront.Models.User;

namespace BlogFront.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly UserStateManager _userState;
        private readonly ISnackbar _snackbar;

        public AuthService(HttpClient http, UserStateManager userState, ISnackbar snackbar)
        {
            _http = http;
            _userState = userState;
            _snackbar = snackbar;
        }

        public async Task<string?> SignUp(string email, string password)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/auth/signup", new { Email = email, Password = password });

                if (res.IsSuccessStatusCode)
                {
                    _userState.SetUser(new BlogUser(email));
                    _snackbar.Add("Signup successful! Please check your email to verify your account.", Severity.Success);
                    return null;
                }

                var error = await res.Content.ReadAsStringAsync();

                if (error.Contains("rate limit", StringComparison.OrdinalIgnoreCase))
                    _snackbar.Add("Too many attempts. Please wait a few minutes before trying again.", Severity.Warning);
                else if (error.Contains("password"))
                    _snackbar.Add("Password must be at least 6 characters.", Severity.Error);
                else if (error.Contains("email"))
                    _snackbar.Add("Email is already registered. Try logging in.", Severity.Warning);
                else
                    _snackbar.Add($"Signup failed: {error}", Severity.Error);

                return error;
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Signup failed: {ex.Message}", Severity.Error);
                return ex.Message;
            }
        }

        public async Task<string?> SignIn(string email, string password)
        {
            try
            {
                var res = await _http.PostAsJsonAsync("api/auth/signin", new { Email = email, Password = password });

                if (res.IsSuccessStatusCode)
                {
                    // Deserialize JSON into SupabaseSessionResponse
                    var session = await res.Content.ReadFromJsonAsync<SupabaseSessionResponse>();

                    if (session?.User != null)
                    {
                        _userState.SetUser(new BlogUser(session.User.Email ?? email));
                        _snackbar.Add("Login successful!", Severity.Success);
                        return null;
                    }

                    _snackbar.Add("Login failed: No valid session.", Severity.Error);
                    return "No valid session";
                }

                var error = await res.Content.ReadAsStringAsync();

                if (error.Contains("Invalid login credentials", StringComparison.OrdinalIgnoreCase))
                    _snackbar.Add("Invalid login credentials", Severity.Error);
                else if (error.Contains("Email not verified", StringComparison.OrdinalIgnoreCase))
                    _snackbar.Add("Email not verified. Please check your inbox.", Severity.Warning);
                else
                    _snackbar.Add($"Login failed: {error}", Severity.Error);

                return error;
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Login failed: {ex.Message}", Severity.Error);
                return ex.Message;
            }
        }

        public async Task SignOut()
        {
            try
            {
                await _http.PostAsync("api/auth/signout", null);
                _userState.ClearUser();
                _snackbar.Add("User signed out successfully.", Severity.Success);
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Sign out failed: {ex.Message}", Severity.Error);
            }
        }

        public async Task<string?> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            try
            {
                var request = new { Email = email, CurrentPassword = currentPassword, NewPassword = newPassword };
                var response = await _http.PostAsJsonAsync("api/auth/change-password", request);

                if (response.IsSuccessStatusCode)
                {
                    _snackbar.Add("Password updated successfully.", Severity.Success);
                    return null;
                }

                var error = await response.Content.ReadAsStringAsync();

                if (error.Contains("Invalid current password", StringComparison.OrdinalIgnoreCase))
                    _snackbar.Add("Your current password is incorrect.", Severity.Error);
                else
                    _snackbar.Add($"Failed to change password: {error}", Severity.Error);

                return error;
            }
            catch (Exception ex)
            {
                _snackbar.Add($"Error: {ex.Message}", Severity.Error);
                return ex.Message;
            }
        }
    }
}
