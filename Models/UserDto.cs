
namespace IdentityServer.Models
{
    public class UserDto
    {

        public string Username { get; set; }
        public string Email { get; set; }
        public string EncryptedPassword { get; set; }
        public string Role { get; set; }
    }
}

