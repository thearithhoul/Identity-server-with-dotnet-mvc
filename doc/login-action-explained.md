# Login Action Explained (MVC)

This describes what the `Login` POST action does step by step.

## What the action receives
`Login(LoginViewModel model)` receives:
- `Username`
- `Password`
- `ReturnUrl` (optional)

Model validation (`[Required]`) is checked first.

## Step-by-step logic
1. **Validate the model**  
   If required fields are missing, it returns the same view with validation errors.

2. **Read credentials from configuration**  
   It reads:
   - `AdminCredentials:Username`
   - `AdminCredentials:Password`
   If config is missing, it falls back to `admin/admin`.

3. **Check username and password**  
   It compares the submitted values to the expected ones.  
   If they don’t match, it returns the view with an error message.

4. **Create an auth principal**  
   It builds a `ClaimsIdentity` with a single claim (`ClaimTypes.Name`) and wraps it in a `ClaimsPrincipal`.

5. **Sign in with cookies**  
   `SignInAsync` issues the authentication cookie using the cookie scheme.

6. **Redirect the user**  
   - If `ReturnUrl` is set and is local, it redirects there.  
   - Otherwise, it redirects to the site root (`/`).

## Why this works
After `SignInAsync`, the auth cookie is stored in the browser.  
Any MVC action protected by `[Authorize]` will then allow access.
