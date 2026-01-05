using System.Security.Claims;

namespace IdentityServer.Interface;

public interface ICustomerIdentityProvider
{
    string? GetUserId(ClaimsPrincipal user);
    bool IsInRole(ClaimsPrincipal user, string role);
    bool IsAuthenticated(ClaimsPrincipal user);
}
