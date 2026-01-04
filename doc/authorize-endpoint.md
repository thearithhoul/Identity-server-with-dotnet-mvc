# /connect/authorize Endpoint (Logic Guide)

This endpoint is the entry point for the Authorization Code flow. It should only decide **who the user is** and **what claims/scopes** to issue, then let OpenIddict generate the code and redirect.

## High-level flow
1. Read the OpenID Connect request from the current HTTP context.
2. If the user is **not signed in**, redirect to the login page (challenge).
3. If the user **is signed in**, build a `ClaimsPrincipal` for OpenIddict.
4. Apply requested scopes and resources.
5. Set claim destinations (ID token, access token).
6. Return `SignIn(...)` to let OpenIddict issue the authorization code.

## Step-by-step logic
1. **Read the request**
   - Use OpenIddict helpers to read the incoming `/connect/authorize` request.
   - If you cannot read the request, return a server error.

2. **Check authentication**
   - If the user is not authenticated, call `Challenge(...)`.
   - Include the current URL (path + query) so the user returns after login.

3. **Create the OpenIddict principal**
   - Create a `ClaimsIdentity` with at least:
     - `sub` (subject) = unique user id
     - `name` or any other user info you want
   - Wrap it in a `ClaimsPrincipal`.

4. **Apply scopes and resources**
   - Use the scopes requested in the authorize request.
   - If you issue an API access token, set the resource name (e.g., `"api"`).

5. **Set claim destinations**
   - Decide which claims go into the ID token vs. access token.
   - Example: `email` only in ID token when `scope=email` is present.

6. **Return `SignIn`**
   - `SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)`
   - OpenIddict handles the rest (authorization code creation + redirect).

## Minimal pseudocode
```text
request = GetOpenIddictRequest()
if not authenticated:
    Challenge(loginPath, returnUrl = currentUrl)

claims = [sub, name, email?]
principal = new ClaimsPrincipal(identity)
principal.SetScopes(request.Scopes)
principal.SetResources("api")
set claim destinations

return SignIn(principal, openiddictScheme)
```

## Notes
- The `sub` (subject) claim is required for OIDC.
- `redirect_uri` must match a seeded redirect URI exactly.
- If scopes are missing or not permitted, OpenIddict will reject the request.
