using BlogFront.Services;
using System;
using GotrueUser = Supabase.Gotrue.User; // 👈 ALIAS
using BlogUser = BlogFront.Models.User;  // 👈 ALIAS

namespace BlogFront.Services
{
    public class UserStateManager
    {
        public BlogUser? User { get; private set; }

        public string? UserEmail => User?.Email;
        public string? UserName => User?.Name;

        public bool IsAdmin => User?.IsAdmin ?? false;
        public bool IsSuperAdmin => User?.IsSuperAdmin ?? false;

        // ✅ New property\
        public bool IsEmailConfirmed => User?.EmailConfirmed ?? false;


        public event Action? OnChange;

        public void SetUser(BlogUser? user) // 👈 updated to BlogUser
        {
            User = user;
            NotifyStateChanged();
            Console.WriteLine($"User state updated: {user?.Email} (Confirmed: {IsEmailConfirmed})");
        }

        public void ClearUser()
        {
            User = null;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        public bool IsLoggedIn() => User != null ;

    }
}
