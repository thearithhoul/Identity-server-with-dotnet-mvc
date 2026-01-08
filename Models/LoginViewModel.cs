using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models;


public class LoginViewModel
{
    [Required(ErrorMessage = "RequiredField")]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "RequiredField")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;


    public string? ReturnUrl { get; set; }
}
