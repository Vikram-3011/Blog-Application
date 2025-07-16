using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Supabase;
using BlogBack.Models;
using static Supabase.Postgrest.Constants;
using BlogBack.Services;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly Supabase.Client _client;
    private readonly AdminService _adminService;
    public AdminController(IOptions<SupabaseConfig> config)
    {
        var options = new SupabaseOptions { AutoConnectRealtime = false };
        _client = new Supabase.Client(config.Value.Url, config.Value.ServiceRoleKey, options);
        _client.InitializeAsync().Wait();
        _adminService = new AdminService(_client); // ✅ Initialize your service

    }

    [HttpGet("is-super-admin/{email}")]
    public async Task<IActionResult> IsSuperAdmin(string email)
    {
        var service = new AdminService(_client);
        bool isSuper = await service.IsSuperAdmin(email);
        return Ok(isSuper);
    }

    [HttpGet("get-current-email")]
    public IActionResult GetCurrentEmail()
    {
        var email = User?.Identity?.Name;
        return Ok(email);
    }

    [HttpGet("is-admin/{email}")]
    public async Task<IActionResult> IsAdmin(string email)
    {
        email = email.ToLower().Trim();          // normalise

        // Optional safety‑check
        await _client.InitializeAsync(); // Safe to call multiple times


        var result = await _client
            .From<Admin>()
            .Filter("email", Operator.Equals, email)
            .Get();

        bool isAdmin = result.Models.Any();
        return Ok(isAdmin);
    }
    [HttpPost("promote")]
    public async Task<IActionResult> Promote([FromQuery] string targetEmail, [FromQuery] string requesterEmail)
    {
        var service = new AdminService(_client);
        if (!await service.IsSuperAdmin(requesterEmail))
            return Forbid("Only super admin can promote users.");

        await service.PromoteAsync(targetEmail);
        return Ok();
    }

    [HttpPost("demote")]
    public async Task<IActionResult> Demote([FromQuery] string targetEmail, [FromQuery] string requesterEmail)
    {
        var service = new AdminService(_client);
        if (!await service.IsSuperAdmin(requesterEmail))
            return Forbid("Only super admin can demote users.");

        await service.DemoteAsync(targetEmail);
        return Ok();
    }




}
