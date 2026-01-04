# OpenIddict AddServer: What to Configure

Use `AddServer` to define the OAuth/OIDC endpoints, allowed flows, tokens, and ASP.NET Core integration.

## Minimum configuration (Authorization Code flow)
```csharp
builder.Services.AddOpenIddict()
    .AddServer(options =>
    {
        // Endpoints
        options.SetAuthorizationEndpointUris("/authorize")
               .SetTokenEndpointUris("/token")
               .SetEndSessionEndpointUris("/logout");

        // Flow
        options.AllowAuthorizationCodeFlow();

        // Scopes your clients can request
        options.RegisterScopes("openid", "profile", "email", "api");

        // Dev certificates (replace with real certs in production)
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        // ASP.NET Core integration
        options.UseAspNetCore()
               .EnableAuthorizationEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough();
    });
```

## What each part does
- **Endpoints**: URLs your clients call (`/authorize`, `/token`, optional `/logout`).
- **Flows**: Which OAuth/OIDC flows are allowed (here: Authorization Code).
- **Scopes**: Claims your apps can request (must match seeded clients).
- **Credentials**: Signing/encryption keys for tokens.
- **UseAspNetCore**: Hooks OpenIddict into ASP.NET Core and lets you handle endpoints.

## When you need to change this
- Add new scopes for APIs or user claims.
- Enable refresh tokens (add `options.AllowRefreshTokenFlow()`).
- Set a custom issuer if behind a reverse proxy.
- Replace dev certs with persisted certs for production.

## Next steps after AddServer
1. Implement `/authorize` endpoint to sign in users and issue a principal.
2. Seed client apps/scopes in the database.
3. Add OpenIddict validation only if this same app will validate access tokens.
