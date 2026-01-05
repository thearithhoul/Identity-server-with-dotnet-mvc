using IdentityServer.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.API
{
    public class IdpContext : DbContext
    {
        public IdpContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.UseOpenIddict();
        }
        public DbSet<User> Users { get; set; }
    }
}