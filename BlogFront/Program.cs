using BlogFront;
using BlogFront.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http.Json; // for GetFromJsonAsync

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<UserStateManager>();
builder.Services.AddScoped<BlogService>();
builder.Services.AddScoped<ProfileService>();
builder.Services.AddScoped<AdminServiceFront>();

// Load appsettings.json from wwwroot
using var http = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
var config = await http.GetFromJsonAsync<AppSettings>("appsettings.json");

if (config is null || string.IsNullOrWhiteSpace(config.ApiBaseUrl))
{
    throw new Exception("ApiBaseUrl is missing in appsettings.json");
}


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(config.ApiBaseUrl) });

await builder.Build().RunAsync();

// Config model
public class AppSettings
{
    public string ApiBaseUrl { get; set; }
}
