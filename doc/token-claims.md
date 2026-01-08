# Token claims (what they mean)

This project issues access tokens with standard JWT/OIDC claims plus a few identity claims. Below is what each claim means and why you might include it.

## Standard JWT/OIDC claims
- `sub`: Subject (unique user identifier). Required for OIDC.
- `iss`: Issuer (your IdentityServer URL).
- `aud`: Audience (the API this token is intended for).
- `exp`: Expiration time (Unix timestamp).
- `nbf`: Not valid before (Unix timestamp).
- `iat`: Issued at (Unix timestamp).

## Identity / profile claims
- `name`: Display name.
- `given_name`: First name (if used).
- `unique_name`: Unique username.
- `email`: Email address.
- `role`: User role(s) used by `[Authorize(Roles = "...")]`.

## Custom claims
- `sso_token`: Custom claim that contains a nested token/string.
  This can make access tokens very large and is usually unnecessary for APIs.

## How claims get into the access token
Claims are added in `Controllers/AuthorizationController.cs` and only included in the
access token if their destination includes `Destinations.AccessToken`.

Tip: Keep access tokens small by only including claims your API actually needs.
