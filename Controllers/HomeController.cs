using IdentityServer.API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Authorize()]
public class HomeController : Controller
{
    private readonly IConfiguration _config;
    // private readonly IdpContext _idpContext;
    public HomeController(IConfiguration configuration)
    {
        _config = configuration;
        // _idpContext = idpContext;u

    }

    [HttpGet("/")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RedirectLink(string id)
    {

        var url = string.Empty;
        switch (id)
        {
            case "arSystem":
                url = _config["appinfo:ArSystem"];
                break;
            case "financeSystem":
                url = "#";
                break;
            case "stockSystem":
                url = "#";
                break;
            case "poSystem":
                url = "#";
                break;
            case "archSystem":
                url = "#";
                break;
            case "hrSystem":
                url = "#";
                break;
            default:
                url = "#";
                break;
        }

        if (string.IsNullOrWhiteSpace(url))
        {

            return RedirectToAction(nameof(Index));
        }

        return Redirect(url);
    }
}
