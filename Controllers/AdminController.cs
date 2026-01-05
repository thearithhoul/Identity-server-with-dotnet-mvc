using IdentityServer.Interface;
using IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly ICustomerIdentityProvider _identityProvider;

    public AdminController(ICustomerIdentityProvider identityProvider)
    {
        _identityProvider = identityProvider;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost("adduser")]
    public async Task<IActionResult> AddNewUser([FromBody] UserDto user)
    {
        try
        {
            var creatorId = _identityProvider.GetUserId(User);
            if (string.IsNullOrWhiteSpace(creatorId))
            {
                return Unauthorized();
            }

            if (!_identityProvider.IsInRole(User, "Admin"))
            {
                return Forbid();
            }


            return Ok();
        }
        catch (System.Exception)
        {
            return BadRequest();
            throw;
        }

    }
}
