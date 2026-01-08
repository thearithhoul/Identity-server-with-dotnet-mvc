using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;

public class SeedClientViewModel
{
    [Required]
    public string ClientId { get; set; } = string.Empty;

    public string? ClientSecret { get; set; }

    public string? DisplayName { get; set; }

    [Required]
    [RegularExpression(@"^https?://\S+$", ErrorMessage = "Redirect URI must be an absolute http/https URL.")]
    public string RedirectUri { get; set; } = string.Empty;

    [RegularExpression(@"^https?://\S+$", ErrorMessage = "Post logout redirect URI must be an absolute http/https URL.")]
    public string? PostLogoutRedirectUri { get; set; }

    public string? Scopes { get; set; }
}
