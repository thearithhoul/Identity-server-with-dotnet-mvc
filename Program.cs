using IdentityServer.API;
using IdentityServer.Interface;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using IdentityServer.Entities;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using IdentityServer.Models;

var builder = WebApplication.CreateBuilder(args);

var supporttedCultures = new[]
{
    new CultureInfo("en-US"),
    new CultureInfo("km-KH"),
};

var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-Us"),
    SupportedCultures = supporttedCultures,
    SupportedUICultures = supporttedCultures,
    ApplyCurrentCultureToResponseHeaders = true
};


builder.Services.AddLocalization(options =>
{
    options.ResourcesPath = "Resources";
});


// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
});
builder.Services.AddControllersWithViews()
.AddViewLocalization()
.AddDataAnnotationsLocalization(options =>
{
    options.DataAnnotationLocalizerProvider = (type, factory) =>
        factory.Create(typeof(IdentityServer.SharedResource));
});



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<ICustomerIdentityProvider, CustomerIdentityProvider>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddOpenIddict().AddCore(
    options =>
    {
        options.UseEntityFrameworkCore().UseDbContext<IdpContext>();
    }
);

builder.Services.AddDbContext<IdpContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.UseOpenIddict();
});

builder.Services.AddOpenIddict()
.AddServer(
    options =>
    {
        options.SetAuthorizationEndpointUris("/connect/authorize")
        .SetTokenEndpointUris("/connect/token")
        .SetEndSessionEndpointUris("/connect/logout")
        .SetUserInfoEndpointUris("/connect/userinfo");

        options.AllowAuthorizationCodeFlow();

        options.RegisterScopes("openid", "profile", "email", "api");

        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

        options.UseAspNetCore()
               .DisableTransportSecurityRequirement()
               .EnableAuthorizationEndpointPassthrough()
               .EnableEndSessionEndpointPassthrough()
               .EnableUserInfoEndpointPassthrough();


    }
);
builder.Services.AddScoped<CustomerIdentityProvider>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRequestLocalization(localizationOptions);

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages()
   .WithStaticAssets();

app.Run();
