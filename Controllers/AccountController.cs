using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using IdentityServer.Entities;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using OpenIddict.Server.AspNetCore;


namespace IdentityServer.Controllers;

public class AccountController : Controller
{
    private readonly IStringLocalizer<AccountController> _localizer;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AccountController(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;

    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)

    {
        Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(CultureInfo.CurrentCulture)));
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult SetLanguage(string culture, string? returnUrl = null)
    {


        if (string.IsNullOrWhiteSpace(culture))
        {
            return RedirectToAction(nameof(Login));
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await GetUserAsync(model.Username);

        if (user == null)
        {
            var currentUi = CultureInfo.CurrentUICulture.Name;
            if (currentUi == "km-KH")
            {
                ModelState.AddModelError(string.Empty, "ឈ្មោះអ្នកប្រើ ឬ ពាក្យសម្ងាត់ មិនត្រឹមត្រូវ");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
            }

            return View(model);
        }

        var passwordResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.EncryptedPassword,
            model.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
        {
            var legacyHash = LegacyHashPassword(model.Password);
            if (!FixedTimeEquals(user.EncryptedPassword, legacyHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var upgradedHash = _passwordHasher.HashPassword(user, model.Password);
            await _userRepository.UpdatePasswordAsync(user.UserId, upgradedHash);
        }
        else if (passwordResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            var upgradedHash = _passwordHasher.HashPassword(user, model.Password);
            await _userRepository.UpdatePasswordAsync(user.UserId, upgradedHash);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return LocalRedirect(model.ReturnUrl);
        }

        return Redirect("~/");
    }

    [Authorize]
    [HttpGet("/connect/logout")]
    public IActionResult Logout()
    {
        var redirectUri = Url.Action(nameof(Login), "Account") ?? "/Account/Login";
        return SignOut(
            new AuthenticationProperties { RedirectUri = redirectUri },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }

    // [Authorize]
    // [HttpPost("/connect/logout")]
    // [ValidateAntiForgeryToken]
    // public IActionResult LogoutPost()
    // {
    //     var redirectUri = Url.Action(nameof(Login), "Account") ?? "/Account/Login";
    //     return SignOut(
    //         new AuthenticationProperties { RedirectUri = redirectUri },
    //         CookieAuthenticationDefaults.AuthenticationScheme,
    //         OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    // }

    private static string LegacyHashPassword(string password)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool FixedTimeEquals(string left, string right)
    {
        if (left.Length != right.Length)
        {
            return false;
        }

        var leftBytes = System.Text.Encoding.UTF8.GetBytes(left);
        var rightBytes = System.Text.Encoding.UTF8.GetBytes(right);
        return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
    }

    private async Task<User?> GetUserAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        var loginId = username.Trim();
        var isEmail = Regex.IsMatch(
            loginId,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.CultureInvariant);

        try
        {
            return await _userRepository.GetUserAsync(
                username: isEmail ? null : loginId,
                email: isEmail ? loginId : null);
        }
        catch (KeyNotFoundException)
        {
            return null;
        }
    }
}
