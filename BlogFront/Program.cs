using BlogFront;
using BlogFront.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<UserStateManager>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AdminServiceFront>();

// Directly set the backend API URL here (Render backend URL)
var apiBaseUrl = "https://localhost:7297/"; // <-- change this to your real backend Render URL

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl)
});

await builder.Build().RunAsync();
