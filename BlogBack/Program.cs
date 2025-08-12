using BlogBack.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Supabase;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<MongoBlogService>();
builder.Services.AddSingleton<MongoUserProfileService>();


builder.Services.Configure<SupabaseConfig>(builder.Configuration.GetSection("SupabaseConfig"));

builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IOptions<SupabaseConfig>>().Value;

    var client = new Client(
        cfg.Url,
        cfg.ApiKey,
        new SupabaseOptions { AutoConnectRealtime = false });

    client.InitializeAsync().Wait();
    return client;
});

builder.Services.AddScoped<AdminService>();        // <— our helper service
builder.Services.AddHttpContextAccessor();         // required to read caller email


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{builder.Configuration["SupabaseConfig:Url"]}/auth/v1";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,  // Supabase handles issuer internally
            ValidateAudience = false,
            NameClaimType = "email" // exposes caller email as User.Identity.Name
        };
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://blog-application-6-7axi.onrender.com/")
              .AllowAnyHeader()
              .AllowAnyMethod();

    });
});
var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


// ? Add this root endpoint to avoid 404 on `/`
app.MapGet("/", () => Results.Ok("Backend is running successfully on Render!"));

app.Run();
