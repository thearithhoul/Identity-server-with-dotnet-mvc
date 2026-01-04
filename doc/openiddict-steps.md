# OpenIddict: Setup Steps and Key Concepts

This guide focuses on using OpenIddict as a self-hosted OAuth 2.0 / OpenID Connect (OIDC) provider for server-side web apps.

## What you need to know about OpenIddict
- OpenIddict is a framework, not a hosted IdP. You build the IdP app (UI, user store, consent) yourself.
- It integrates deeply with ASP.NET Core authentication/authorization.
- You choose storage: EF Core (SQL Server, PostgreSQL, etc.) or other custom stores.
- You control tokens (JWT vs reference), scopes, lifetimes, and endpoints.
- You must seed and manage client applications, scopes, and signing credentials.

## High-level steps (server-side web apps)
1. Create an ASP.NET Core web app for the IdP.
2. Add OpenIddict packages:
   - `OpenIddict`
   - `OpenIddict.EntityFrameworkCore`
   - `OpenIddict.AspNetCore`
3. Create a DbContext and enable OpenIddict stores.
4. Register OpenIddict in `Program.cs` with core, server, and (optional) validation.
5. Configure endpoints and allowed flows (Authorization Code).
6. Add signing and encryption credentials.
7. Build login/consent UI and return a claims principal.
8. Seed client apps and scopes.
9. Run migrations and test `/authorize` and `/token`.

## Example service configuration (outline)
```csharp
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AuthDbContext>();
    })
    .AddServer(options =>
    {
        options.SetAuthorizationEndpointUris("/authorize")
               .SetTokenEndpointUris("/token");
               
        options.AllowAuthorizationCodeFlow();

        options.RegisterScopes("openid", "profile", "email", "api");

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });
```

## Authorization endpoint handler (outline)
You must authenticate the user and issue a principal:
```csharp
[HttpGet("/authorize")]
public IActionResult Authorize()
{
    // If user not signed in, redirect to login.
    // If signed in, build claims principal and return SignIn(...).
}
```

## Token endpoint
OpenIddict handles `/token` when you enable the passthrough.
Your job is to validate client credentials (confidential clients) and issue claims.

## Seeding a client app (outline)
Use `IOpenIddictApplicationManager` at startup to register your web app client:
```csharp
await manager.CreateAsync(new OpenIddictApplicationDescriptor
{
    ClientId = "web-app",
    ClientSecret = "secret",
    ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
    DisplayName = "Web App",
    RedirectUris = { new Uri("https://app.example.com/callback") },
    PostLogoutRedirectUris = { new Uri("https://app.example.com/signout-callback") },
    Permissions =
    {
        OpenIddictConstants.Permissions.Endpoints.Authorization,
        OpenIddictConstants.Permissions.Endpoints.Token,
        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
        OpenIddictConstants.Permissions.ResponseTypes.Code,
        OpenIddictConstants.Permissions.Scopes.OpenId,
        OpenIddictConstants.Permissions.Scopes.Profile,
        OpenIddictConstants.Permissions.Scopes.Email
    }
});
```

## Key decisions to make early
- Storage provider (EF Core with SQL Server, PostgreSQL, etc.).
- Token format: JWT vs reference tokens.
- Scopes and claim mapping.
- Consent UX (implicit vs explicit).
- Client types (confidential vs public).

## Operational checklist
- Rotate signing keys and back them up.
- Set proper token lifetimes.
- Enforce HTTPS and secure cookies.
- Log OpenIddict events for troubleshooting.

## Common gotchas
- Missing redirect URI or scope permissions will cause `/authorize` to fail.
- Not seeding clients before testing causes 400 errors.
- Misconfigured endpoints or missing passthrough result in blank responses.
