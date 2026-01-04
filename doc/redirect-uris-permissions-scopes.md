# Redirect URIs, Logout Redirect URIs, Permissions, and Scopes

This explains the key fields you see when seeding an OpenIddict client.

## RedirectUris
`RedirectUris` is the list of **allowed callback URLs** for the client.

After a successful login, OpenIddict redirects the browser back to the client with an authorization code:

```
https://app.example.com/callback?code=...&state=...
```
Important rules:
- The `redirect_uri` sent in `/authorize` **must exactly match** one of the seeded URIs.
- Exact means same scheme (`https` vs `http`), host, port, and path.
- If it doesn’t match, OpenIddict returns an error.

## PostLogoutRedirectUris
`PostLogoutRedirectUris` is the list of **allowed logout callback URLs** for the client.

After the user signs out at the IdP, OpenIddict can redirect back to the client:
```
https://app.example.com/signout-callback
```

Important rules:
- The `post_logout_redirect_uri` must exactly match one of these URIs.
- It is optional, but recommended for a smooth logout flow.

## Permissions
Permissions define **what the client is allowed to do**.

Common permissions you set for Authorization Code flow:
- `Permissions.Endpoints.Authorization` (allow `/authorize`)
- `Permissions.Endpoints.Token` (allow `/token`)
- `Permissions.GrantTypes.AuthorizationCode` (allow `grant_type=authorization_code`)
- `Permissions.ResponseTypes.Code` (allow `response_type=code`)
- `Permissions.Scopes.OpenId` / `Profile` / `Email` (allow standard OIDC scopes)

If a permission is missing, OpenIddict rejects the request.

## Scopes
Scopes describe **what data or access** the client is requesting.

Examples:
- `openid`: required for OIDC authentication (issues an ID token)
- `profile`: standard user profile claims
- `email`: user email claim
- `api`: your custom API scope

You must register scopes in your server configuration:
```csharp
options.RegisterScopes("openid", "profile", "email", "api");
```

If a client requests a scope it doesn’t have permission for, the request fails.

## Quick example (seeded client)
```csharp
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
```
