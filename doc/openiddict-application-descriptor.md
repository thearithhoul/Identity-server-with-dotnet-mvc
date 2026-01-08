# OpenIddictApplicationDescriptor fields

Use these fields when defining a client (application). The notes below explain what each field does and when you typically use it.

## Core identity
- `ClientId`: Required. Public identifier for the client. This is the value sent as `client_id`.
  Use for every client.

- `ClientSecret`: Secret for confidential clients. Required for server-side apps and daemon services.
  Do not use for SPAs or native apps.

- `ClientType`: `Public` or `Confidential`. Public clients have no secret.
  Use `Public` for SPAs/mobile/desktop, `Confidential` for server apps.

- `ApplicationType`: High-level app kind (e.g., `web` or `native`).
  Use to describe the app type; not strictly required for all flows.

## Display and localization
- `DisplayName`: Single display name shown in logs or admin UI.
- `DisplayNames`: Localized names keyed by culture (e.g., `en-US`, `ar-EG`).
  Use when you need UI text in multiple languages.

## Consent
- `ConsentType`: How user consent is handled.
  - `Explicit`: prompt the user at least once.
  - `Systematic`: always prompt.
  - `Implicit`: no prompt when already authorized.
  - `External`: consent is handled outside OpenIddict.

## Redirects
- `RedirectUris`: Allowed sign-in callback URLs (required for auth code flow).
  Use exact URIs, including scheme and port.
- `PostLogoutRedirectUris`: Allowed post-logout callback URLs.
  Use for sign-out redirect back to the client.

## Permissions and requirements
- `Permissions`: Allowed endpoints, flows, response types, and scopes.
  Example: authorization endpoint + authorization code flow + `openid profile email api`.
- `Requirements`: Extra security requirements.
  Common one: `ProofKeyForCodeExchange` for public clients (PKCE).

## Keys and advanced auth
- `JsonWebKeySet`: JWKS used for client authentication or JWT-based client assertions.
  Use for private-key client auth or when rotating client keys.

## Custom metadata
- `Properties`: Arbitrary JSON-style metadata for your app (tags, owner, tenant id).
  Not used by OpenIddict itself.
- `Settings`: Additional configuration values stored for the client.
  Use for custom app settings you want to store with the client.

## Internal
- `ConcurrencyToken`: Used by OpenIddict for optimistic concurrency.
  Do not set manually.

## What to set in common cases
- Web app (confidential): `ClientId`, `ClientSecret`, `ClientType=Confidential`, `RedirectUris`, `PostLogoutRedirectUris`, `Permissions`, `ConsentType`.
- SPA or mobile app (public): `ClientId`, `ClientType=Public`, `RedirectUris`, `Permissions`, `Requirements` (PKCE).
- Machine-to-machine: `ClientId`, `ClientSecret`, `ClientType=Confidential`, `Permissions` (token endpoint + client credentials + scopes).
