using Microsoft.EntityFrameworkCore;

namespace DND.Data.Identity.Initializers
{
    public class IdentityContextInitializerMigrate
    {
        private readonly IdentityContext _context;
        //private readonly UserManager<User> _userManager;
        //private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityContextInitializerMigrate(
            IdentityContext context)
        {
            _context = context;
        }

        //This example just creates an Administrator role and one Admin users
        public void Initialize()
        {
            //create database schema if none exists
            _context.Database.Migrate();

            _context.Seed();

            _context.SaveChanges();
        }
    }
}
