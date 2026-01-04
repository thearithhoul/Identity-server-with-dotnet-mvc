# Next Steps for Your OpenIddict Setup

This list is based on your current project (Razor Pages, EF Core + SQL Server, OpenIddict Core only).

## 1) Add OpenIddict Server configuration
You only registered Core. Add Server with endpoints, flow, scopes, and ASP.NET Core integration:
- `/authorize` and `/token` endpoints
- Authorization Code flow
- Scopes you plan to issue
- Dev signing/encryption certs (replace in production)

## 2) Add authentication + middleware
You need a local sign-in mechanism for the IdP app:
- Add cookie authentication in services
- Add `app.UseAuthentication()` before `app.UseAuthorization()`

## 3) Update the DbContext for OpenIddict
Your `IdpContext` is empty. You should either:
- Inherit from OpenIddict's EF Core context, or
- Override `OnModelCreating` and call `modelBuilder.UseOpenIddict()`

Without this, migrations won't create the OpenIddict tables.

## 4) Create login + consent pages
Because this is an IdP, you must build the user sign-in UI:
- Login page (Razor Pages)
- Optional consent page

The `/authorize` endpoint should:
- Redirect to login if user not signed in
- Create a claims principal and return `SignIn(...)` when signed in

## 5) Seed clients and scopes
Register your web app clients at startup using:
- `IOpenIddictApplicationManager`
- `IOpenIddictScopeManager`

If you skip seeding, `/authorize` will return errors.

## 6) Add EF migrations and create the database
Add and apply migrations once the context is correct.
Typical commands:
- `dotnet ef migrations add OpenIddictInit -c IdpContext`
- `dotnet ef database update`

## 7) Fix HTTPS for development
OpenIddict requires HTTPS by default.
Choose one:
- Use HTTPS dev cert and `https://localhost:xxxx`
- OR disable HTTPS requirement for dev via `UseAspNetCore().DisableTransportSecurityRequirement()`

## 8) Test the flow
Start with a simple `/authorize` request, then exchange the code at `/token`.
Use the same redirect URI and scopes you seeded.

## Optional: Add token validation
Only add OpenIddict validation if this same app will validate access tokens.
