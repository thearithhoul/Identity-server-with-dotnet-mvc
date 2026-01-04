# Scopes and Roles (How They Differ)

Scopes and roles are both used for access control, but they solve **different problems**.

## Scopes (OAuth / OIDC)
Scopes describe **what the client is allowed to do** or **what data it can access**.

Examples:
- `openid`: required for OIDC login (ID token)
- `profile`: basic profile claims (name, etc.)
- `email`: email claim
- `api`: access to your API

Where scopes are used:
- Requested by the client in `/connect/authorize`
- Validated by the server (OpenIddict)
- Included in tokens as permissions

Why scopes matter:
- They control which claims go into the ID token.
- They control which resources (APIs) the access token can be used for.

## Roles (Application Authorization)
Roles describe **who the user is in your application** (e.g., Admin, Manager, User).

Examples:
- `Admin`
- `Manager`
- `Support`

Where roles are used:
- Stored on the user (database or identity system)
- Added as claims during login
- Used by ASP.NET Core `[Authorize(Roles = "Admin")]`

Why roles matter:
- They control which UI/pages/actions a user can access.
- They are not part of OAuth by default (unless you add them as claims).

## How they work together
- Scopes limit **client access**.
- Roles limit **user access**.

Example:
- A client gets `scope=api`, so it can call your API.
- A user has `role=Admin`, so they can call admin endpoints inside that API.

## Should roles go into tokens?
Sometimes yes, sometimes no:
- Add roles to access tokens if your API needs them.
- Do not add roles to ID tokens unless the client UI needs them.

## Simple rule of thumb
- Scopes = what the **client** can ask for.
- Roles = what the **user** is allowed to do.
