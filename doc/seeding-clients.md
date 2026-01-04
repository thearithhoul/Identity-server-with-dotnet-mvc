# Seeding Clients in OpenIddict

Seeding a client means creating the client application record in the OpenIddict database so the server can recognize it by `client_id`. Without this, `/authorize` or `/token` requests fail with "invalid client".

## Why it matters
- OpenIddict validates `client_id` against its database.
- The client record stores redirect URIs, permissions, and secrets.
- Seeding is required before the first login/test.

## Step-by-step (recommended)
1. Create a startup seed block that runs once when the app starts.
2. Use `IOpenIddictApplicationManager` to check for the client by `ClientId`.
3. If not found, create it with the correct redirect URI and permissions.
4. Run the app once so the client record is created.

## Example seed code (Program.cs)
```csharp
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

// After app is built:
using var scope = app.Services.CreateScope();
var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

if (await manager.FindByClientIdAsync("web-app") is null)
{
    await manager.CreateAsync(new OpenIddictApplicationDescriptor
    {
        ClientId = "web-app",
        ClientSecret = "secret", // omit if public client
        DisplayName = "Web App",
        RedirectUris = { new Uri("https://app.example.com/callback") },
        PostLogoutRedirectUris = { new Uri("https://app.example.com/signout-callback") },
        Permissions =
        {
            Permissions.Endpoints.Authorization,
            Permissions.Endpoints.Token,
            Permissions.GrantTypes.AuthorizationCode,
            Permissions.ResponseTypes.Code,
            Permissions.Scopes.OpenId,
            Permissions.Scopes.Profile,
            Permissions.Scopes.Email
        }
    });
}
```

## Common mistakes
- `redirect_uri` in the request does not exactly match the seeded URI.
- Client not created because the database or migrations were not applied.
- Missing permission (e.g., no Authorization endpoint permission).

## Quick verification
- Query the `OpenIddictApplications` table and confirm the `ClientId` is present.
- Try `/authorize` again with the same `client_id` and `redirect_uri`.
