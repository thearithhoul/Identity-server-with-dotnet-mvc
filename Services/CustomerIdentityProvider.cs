using System.Security.Claims;
using IdentityServer.Interface;

namespace IdentityServer.Services;

public class CustomerIdentityProvider : ICustomerIdentityProvider
{
    public string? GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public bool IsInRole(ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    public bool IsAuthenticated(ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated ?? false;
    }
}
