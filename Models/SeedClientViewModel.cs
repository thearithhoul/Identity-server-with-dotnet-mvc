using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

public class SeedClientViewModel
{
    [Required]
    public string ClientId { get; set; } = string.Empty;

    public string? ClientSecret { get; set; }

    public string? DisplayName { get; set; }

    [Required]
    [Url]
    public string RedirectUri { get; set; } = string.Empty;

    [Url]
    public string? PostLogoutRedirectUri { get; set; }

    public string? Scopes { get; set; }
}
