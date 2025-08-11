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
builder.Services.AddScoped<AdminServiceFront>();   // ⬅️ add this



builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://blog-application-2-qnhw.onrender.com/   
") });

await builder.Build().RunAsync();
