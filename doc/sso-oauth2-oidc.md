# SSO with OAuth 2.0 and OpenID Connect (OIDC)

SSO (Single Sign-On) lets a user authenticate once with an Identity Provider (IdP) and then access multiple apps without signing in again. OAuth 2.0 is an authorization framework for accessing APIs; OpenID Connect adds authentication on top of OAuth 2.0 so apps can verify who the user is. Together, they are the common way to implement web SSO.

## Actors
- User (resource owner)
- Client (the app the user is trying to access)
- Authorization Server / Identity Provider (IdP)
- Resource Server (API)

## Building blocks
- Authorization code: temporary code returned to the client after login
- Access token: token the client uses to call APIs
- ID token: OIDC token that contains user identity claims
- Refresh token: token used to get new access tokens (optional)

## Typical flow (Authorization Code)
1. User visits App.
2. App redirects the user to the IdP `/authorize` endpoint with `response_type=code`, `scope=openid ...`, `state`, and `nonce`.
3. IdP authenticates the user and establishes an SSO session (usually a cookie).
4. IdP redirects back to the App with an authorization code.
5. App exchanges the code for tokens at the IdP `/token` endpoint.
6. App validates the ID token (issuer, audience, signature, nonce), creates a local session, and uses the access token to call APIs.

## How SSO happens across multiple apps
- The first app creates an IdP session after the user logs in.
- When the user goes to a second app, that app also redirects to the same IdP.
- The IdP sees the existing session and issues new tokens without re-prompting for credentials.
- Each app gets its own tokens, but the user only signs in once.

## OAuth 2.0 vs OIDC
- OAuth 2.0: "Can this app access a resource?"
- OIDC: "Who is the user?" (adds the ID token and standard user claims)

## Simple request path (ASCII)
User -> App -> IdP -> App -> API
        redirect  authorize  code  token  access_token

## Common security notes
- Always validate `state` and `nonce`.
- Check token signature, issuer, audience, and expiration.
- Use HTTPS everywhere.

## Glossary
- IdP: Identity Provider, the system that authenticates the user.
- Client: App relying on the IdP for login.
- Resource Server: API protected by access tokens.
