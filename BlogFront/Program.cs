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



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7297/") });

await builder.Build().RunAsync();
