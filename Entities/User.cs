
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Entities
{
    public class User
    {
        [Key]
        public int Id {get;set;}
        public Guid UserId {get;set;}
        public string Username {get;set;}
        public string Email {get;set;}
        public string EncryptedPassword {get;set;}
        public string Role {get;set;}
        public DateTime CreatedAt {get;set;}
        public DateTime UpdatedAt {get;set;}
        public bool IsActive {get;set;}
        public string Leng {get; set;}
    }
}