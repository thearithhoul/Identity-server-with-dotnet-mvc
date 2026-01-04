# Authorization Code Flow (OAuth 2.0 / OIDC)

This flow is used by server-side web apps to obtain tokens after the user signs in at the Identity Provider (IdP).

## Endpoints
- Authorization endpoint: `GET /authorize`
- Token endpoint: `POST /token`

## Step-by-step
1. User visits the web app.
2. The app redirects the user to the IdP authorization endpoint.
3. The IdP authenticates the user and asks for consent (if needed).
4. The IdP redirects back to the app with an authorization code.
5. The app exchanges the code for tokens at the token endpoint.
6. The app validates the ID token and creates a local session.

## Authorization request (browser redirect)
```http
GET https://idp.example.com/authorize?
  response_type=code&
  client_id=web-app&
  redirect_uri=https%3A%2F%2Fapp.example.com%2Fcallback&
  scope=openid%20profile%20email&
  state=af0ifjsldkj&
  nonce=n-0S6_WzA2Mj
```

## Authorization response (redirect back to the app)
```http
HTTP/1.1 302 Found
Location: https://app.example.com/callback?code=SplxlOBeZQQYbYS6WxSbIA&state=af0ifjsldkj
```

## Token request (server-to-server)
```http
POST https://idp.example.com/token
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
code=SplxlOBeZQQYbYS6WxSbIA&
redirect_uri=https%3A%2F%2Fapp.example.com%2Fcallback&
client_id=web-app&
client_secret=secret
```

## Token response
```json
{
  "access_token": "eyJ...",
  "id_token": "eyJ...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "refresh_token": "8xLOxBtZp8"
}
```

## Validation checklist
- Verify `state` matches the original request.
- Validate ID token signature, issuer, audience, and `nonce`.
- Enforce HTTPS and secure cookies.

## Minimal sequence (ASCII)
User -> App -> IdP (/authorize) -> App (code) -> IdP (/token) -> App (tokens)
