using System.Security.Claims;
using FhModulplaner.Data;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;

namespace FhModulplaner.Services.Auth;

public class OpenIdConnectEventHandler : OpenIdConnectEvents
{
    
    private readonly AppDbContext _db;
    private readonly ILogger<OpenIdConnectEventHandler> _logger;
    
    public OpenIdConnectEventHandler(AppDbContext db, ILogger<OpenIdConnectEventHandler> logger)
    {
        _db = db;
        _logger = logger;
    }
    
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var givenName = context.Principal?.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
        var surName = context.Principal?.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
        var userAdId = context.Principal?.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier");

        if (userAdId is null)
        {
            throw new Exception("ADId is null");
        }

        var userId = await CheckUser(userAdId, givenName, surName);
        
        var claims = new List<Claim>
        {
            new(AppClaimTypes.UserId, userId.ToString()),
        };
        
        var claimsIdentity = new ClaimsIdentity(claims, "Modulplaner");

        context.Principal?.AddIdentity(claimsIdentity);
    }

    private async Task<Guid> CheckUser(string adUserId, string givenName, string surName)
    {
        var user = await _db.Students.FirstOrDefaultAsync(user => user.ADId == adUserId);

        if (user != null)
        {
            return user.Id;
        }
        
        user = new User
        {
            Id = Guid.NewGuid(),
            ADId = adUserId,
            Name = givenName,
            Surname = surName
        };
            
        await _db.Students.AddAsync(user);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation($"Created new user with id {user.Id}");

        return user.Id;
    }
}

public static class AppClaimTypes
{
    public const string UserId = "Modulplaner-UserId";
}