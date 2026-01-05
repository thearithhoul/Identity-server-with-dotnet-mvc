using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using IdentityServer.API;
using Microsoft.EntityFrameworkCore;


namespace IdentityServer.Controllers;

public class AuthorizationController : Controller
{
    private readonly IdpContext _idpContext;

    public AuthorizationController(
        IdpContext idpContext)
    {
        _idpContext = idpContext;
    }

    [HttpGet("/connect/authorize")]
    [HttpPost("/connect/authorize")]
    public async Task<IActionResult> Authorize()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var user = User.Identity;
        if (!(user?.IsAuthenticated ?? false))
        {
            var returnUrl = Request.PathBase + Request.Path + Request.QueryString;
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl });
        }

        var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
             ?? user.Name
             ?? throw new InvalidOperationException("No user id.");

        var email = User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

        var appUser = await _idpContext.Users
                 .FirstOrDefaultAsync(u => u.UserId.ToString() == subject);

        if (appUser == null || !appUser.IsActive)
        {
            return Forbid(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        // var claims = new List<Claim>
        // {
        //     new Claim(Claims.Subject, appUser.UserId ?? ),
        //     new Claim(Claims.Name, user?.Name ?? userId),
        //     new Claim(Claims.Email, email)
        // };

        var claims = new List<Claim>
        {
            new Claim(Claims.Subject, appUser.UserId.ToString()),
            new Claim(Claims.Name, appUser.Username),
            new Claim(Claims.Email, appUser.Email),
            new Claim(Claims.Role, appUser.Role)
        };
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(claims, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme));

        var allowedScopes = new HashSet<string>(StringComparer.Ordinal)
        {
            Scopes.OpenId,
            Scopes.Profile,
            Scopes.Email,
            "api"
        };

        principal.SetScopes(request.GetScopes().Where(scope => allowedScopes.Contains(scope)));

        if (principal.HasScope("api"))
        {
            principal.SetResources("api");
        }

        foreach (var claim in principal.Claims)
        {
            claim.SetDestinations(GetDestinations(claim, principal));
        }

        return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    private static IEnumerable<string> GetDestinations(Claim claim, ClaimsPrincipal principal)
    {
        switch (claim.Type)
        {
            case Claims.Subject:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;
            case Claims.Name:
                yield return Destinations.AccessToken;
                if (principal.HasScope(Scopes.Profile))
                {
                    yield return Destinations.IdentityToken;
                }
                yield break;
            case Claims.Email:
                yield return Destinations.AccessToken;
                if (principal.HasScope(Scopes.Email))
                {
                    yield return Destinations.IdentityToken;
                }
                yield break;
            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
