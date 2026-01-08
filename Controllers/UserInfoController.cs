using IdentityServer.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers;

[Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
public class UserInfoController : Controller
{
    private readonly IdpContext _idpContext;
    private readonly ILogger<UserInfoController> _logger;

    public UserInfoController(IdpContext idpContext, ILogger<UserInfoController> logger)
    {
        _idpContext = idpContext;
        _logger = logger;
    }

    [HttpGet("/connect/userinfo")]
    [HttpPost("/connect/userinfo")]
    public async Task<IActionResult> UserInfo()
    {
        _logger.LogInformation("UserInfo requested. Authenticated: {IsAuthenticated}", User.Identity?.IsAuthenticated);

        var subject = User.FindFirst(Claims.Subject)?.Value;
        _logger.LogInformation("UserInfo subject: {Subject}", subject);

        if (string.IsNullOrWhiteSpace(subject) || !Guid.TryParse(subject, out var userId))
        {
            _logger.LogWarning("UserInfo rejected. Invalid subject: {Subject}", subject);
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var user = await _idpContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.UserId == userId);

        if (user == null || !user.IsActive)
        {
            _logger.LogWarning("UserInfo rejected. User not found or inactive. UserId: {UserId}", userId);
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        var payload = new Dictionary<string, object?>
        {
            [Claims.Subject] = user.UserId.ToString()
        };

        if (User.HasScope(Scopes.Profile))
        {
            payload[Claims.Name] = user.Username;
        }

        if (User.HasScope(Scopes.Email))
        {
            payload[Claims.Email] = user.Email;
        }

        payload[Claims.Role] = user.Role;

        _logger.LogInformation("UserInfo response built. UserId: {UserId}", userId);
        return Ok(payload);

        // return Ok();
    }
}
