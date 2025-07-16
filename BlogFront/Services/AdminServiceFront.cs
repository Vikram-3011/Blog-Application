using BlogFront.Services;
using System.Net.Http.Json;

public class AdminServiceFront
{
    private readonly HttpClient _http;
    private readonly UserStateManager _userState;
    public AdminServiceFront(HttpClient http, UserStateManager userState)
    {
        _http = http;
        _userState = userState;
    }

    public async Task<bool> IsAdmin(string email) => await _http.GetFromJsonAsync<bool>($"api/admin/is-admin/{Uri.EscapeDataString(email)}");

    public async Task<bool> IsSuperAdmin(string email)
    {
        return await _http.GetFromJsonAsync<bool>($"api/admin/is-super-admin/{Uri.EscapeDataString(email)}");
    }

    public async Task PromoteAsync(string targetEmail)
    {
        var requester = _userState.UserEmail ?? throw new Exception("Requester email is null");
        var url = $"/api/admin/promote?targetEmail={targetEmail}&requesterEmail={requester}";
        var response = await _http.PostAsync(url, null);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task DemoteAsync(string targetEmail)
    {
        var requester = _userState.UserEmail ?? throw new Exception("Requester email is null");
        var url = $"/api/admin/demote?targetEmail={targetEmail}&requesterEmail={requester}";
        var response = await _http.PostAsync(url, null);

        if (!response.IsSuccessStatusCode)
            throw new Exception(await response.Content.ReadAsStringAsync());
    }


}
