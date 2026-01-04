# Claims, Identity, and Principal (Cookie Sign-In)

This explains what happens in this login snippet:

```csharp
var claims = new[] { new Claim(ClaimTypes.Name, model.Username) };
var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
var principal = new ClaimsPrincipal(identity);

await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
```

## Claim
A **claim** is a single piece of identity data.  
Example: `Name = "admin"`.

In OIDC, common claims are:
- `sub` (subject, user id)
- `name`
- `email`

## ClaimsIdentity
`ClaimsIdentity` is a collection of claims plus an **authentication type**.

Why the auth type matters:
- It links the identity to the auth system (here: cookie auth).
- It tells ASP.NET Core which handler created the identity.

## ClaimsPrincipal
`ClaimsPrincipal` wraps one or more identities and represents **the user**.

ASP.NET Core reads the principal from the auth cookie on every request and sets:
```
HttpContext.User
```

## SignInAsync
`SignInAsync` serializes the principal into an auth cookie and sends it to the browser.  
On the next request, the cookie middleware rehydrates the principal and sets `User.Identity.IsAuthenticated = true`.

## Why it matters for OpenIddict
OpenIddict relies on `HttpContext.User` to know who is logged in during `/connect/authorize`.  
So the cookie principal you create here is later used to build the OIDC tokens.
