using Supabase;
//using Supabase.Postgrest;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using static Supabase.Postgrest.Constants;
using BlogBack.Models;          // ⬅️ add

namespace BlogBack.Services;

public class AdminService
{
    private readonly Client _supabase;
   

    public AdminService(Client supabase)
    {
        _supabase = supabase;
        
    }

    public async Task<bool> IsAdminAsync(string email)
    {
        var res = await _supabase.From<Admin>()
                                 .Filter("email", Operator.Equals, email)
                                 .Get();
        return res.Models.Any();
    }

    public Task PromoteAsync(string email) =>
        _supabase.From<Admin>().Insert(new Admin { Email = email });

    public Task DemoteAsync(string email) =>
        _supabase.From<Admin>()
                 .Filter("email", Operator.Equals, email)
                 .Delete();

    public async Task<bool> IsSuperAdmin(string requesterEmail)
    {
        var result = await _supabase
     .From<Admin>()
     .Filter("email", Operator.Equals, requesterEmail.ToLower())
     .Single();

        return result?.IsSuper == true;
    }


}






