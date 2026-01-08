using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace IdentityServer.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin/clients")]
public class ClientsController : Controller
{
    private readonly IOpenIddictApplicationManager _applicationManager;

    public ClientsController(IOpenIddictApplicationManager applicationManager)
    {
        _applicationManager = applicationManager;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var model = new SeedClientViewModel
        {
            Scopes = "openid profile email api"
        };
        return View(model);
    }


    [HttpPost("seed")]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Seed(SeedClientViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("Index", model);
        }

        var existing = await _applicationManager.FindByClientIdAsync(model.ClientId);
        if (existing is not null)
        {
            ModelState.AddModelError(string.Empty, $"Client '{model.ClientId}' already exists.");
            return View("Index", model);
        }

        var redirectUriValue = model.RedirectUri?.Trim();
        if (!Uri.TryCreate(redirectUriValue, UriKind.Absolute, out var redirectUri) ||
            (redirectUri.Scheme != Uri.UriSchemeHttp && redirectUri.Scheme != Uri.UriSchemeHttps))
        {
            ModelState.AddModelError(nameof(model.RedirectUri), "Redirect URI must be an absolute http/https URL.");
            return View("Index", model);
        }

        Uri? postLogoutUri = null;
        if (!string.IsNullOrWhiteSpace(model.PostLogoutRedirectUri) &&
            !Uri.TryCreate(model.PostLogoutRedirectUri, UriKind.Absolute, out postLogoutUri))
        {
            ModelState.AddModelError(nameof(model.PostLogoutRedirectUri), "Post logout redirect URI must be an absolute URL.");
            return View("Index", model);
        }

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = model.ClientId,
            DisplayName = string.IsNullOrWhiteSpace(model.DisplayName) ? model.ClientId : model.DisplayName,
            ConsentType = ConsentTypes.Explicit
        };

        if (!string.IsNullOrWhiteSpace(model.ClientSecret))
        {
            descriptor.ClientSecret = model.ClientSecret;
            descriptor.ClientType = ClientTypes.Confidential;
        }
        else
        {
            descriptor.ClientType = ClientTypes.Public;
        }

        descriptor.RedirectUris.Add(redirectUri);

        if (postLogoutUri is not null)
        {
            descriptor.PostLogoutRedirectUris.Add(postLogoutUri);
        }


        descriptor.Permissions.Add(Permissions.Endpoints.Authorization);
        descriptor.Permissions.Add(Permissions.Endpoints.Token);
        descriptor.Permissions.Add(Permissions.GrantTypes.AuthorizationCode);
        descriptor.Permissions.Add(Permissions.ResponseTypes.Code);

        foreach (var scope in ParseScopes(model.Scopes))
        {
            switch (scope)
            {

                case "profile":
                    descriptor.Permissions.Add(Permissions.Scopes.Profile);
                    break;
                case "email":
                    descriptor.Permissions.Add(Permissions.Scopes.Email);
                    break;
                default:
                    descriptor.Permissions.Add(Permissions.Prefixes.Scope + scope);
                    break;
            }
        }

        await _applicationManager.CreateAsync(descriptor);

        TempData["SeedStatus"] = $"Client '{model.ClientId}' created.";
        return RedirectToAction(nameof(Index));
    }

    private static IEnumerable<string> ParseScopes(string? scopes)
    {
        if (string.IsNullOrWhiteSpace(scopes))
        {
            return Array.Empty<string>();
        }

        return scopes
            .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.Ordinal);
    }
}
