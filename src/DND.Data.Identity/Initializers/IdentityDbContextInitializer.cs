using DND.Domain.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DND.Data.Identity.Initializers
{
    public class IdentityDbContextInitializer
    {
        private readonly IdentityDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public IdentityDbContextInitializer(
            IdentityDbContext context,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        //This example just creates an Administrator role and one Admin users
        public void Initialize()
        {
            //create database schema if none exists
            _context.Database.Migrate();

            ////If there is already an Administrator role, abort
            //if (!_context.Roles.Any(r => r.Name == "admin"))
            //{
            //    _roleManager.CreateAsync(new IdentityRole("admin"));
            //}

            ////Create the default Admin account and apply the Administrator role
            //string user = "admin";
            //string email = "davidikin45@gmail.com";

            //if (_userManager.FindByNameAsync(user).Result == null)
            //{
            //    string password = "password";
            //    var result =  _userManager.CreateAsync(new User { UserName = user, Email = email, EmailConfirmed = true }, password).Result;
            //    var result2 = _userManager.AddToRoleAsync(_userManager.FindByNameAsync(user).Result, "admin").Result;
            //}

            _context.Seed();

            _context.SaveChanges();
        }
    }
}
