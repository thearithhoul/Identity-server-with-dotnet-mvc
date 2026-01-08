using IdentityServer.API;
using IdentityServer.Entities;
using IdentityServer.Interface;
using IdentityServer.Models;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers;

[Authorize(Roles = "Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly ICustomerIdentityProvider _identityProvider;
    private readonly IPasswordHasher<User> _passwordHasher;
    public readonly IUserRepository _userRespository;


    public AdminController(
        ICustomerIdentityProvider identityProvider,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher)
    {
        _identityProvider = identityProvider;
        _userRespository = userRepository;
        _passwordHasher = passwordHasher;
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

            if (string.IsNullOrWhiteSpace(user.EncryptedPassword))
            {
                return BadRequest("Password is required.");
            }

            var convert = new User()
            {
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
            };
            convert.EncryptedPassword = _passwordHasher.HashPassword(convert, user.EncryptedPassword);

            await _userRespository.AddNewUser(convert);

            return Ok("Add Success");
        }
        catch (System.Exception)
        {
            return BadRequest();
            throw;
        }

    }
}
