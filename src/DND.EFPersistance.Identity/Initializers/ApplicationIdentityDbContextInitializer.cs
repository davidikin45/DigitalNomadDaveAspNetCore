using DND.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DND.Common.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.EFPersistance.Identity.Initializers
{
    public class ApplicationIdentityDbContextInitializer
    {
        private readonly ApplicationIdentityDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationIdentityDbContextInitializer(
            ApplicationIdentityDbContext context,
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

            //If there is already an Administrator role, abort
            if (!_context.Roles.Any(r => r.Name == "admin"))
            {
                _roleManager.CreateAsync(new IdentityRole("admin"));
            }

            //Create the default Admin account and apply the Administrator role
            string user = "admin";
            string email = "davidikin45@gmail.com";

            if (_userManager.FindByNameAsync(user).Result == null)
            {
                string password = "PAssword12!";
                var result =  _userManager.CreateAsync(new User { UserName = user, Email = email, EmailConfirmed = true }, password).Result;
                var result2 = _userManager.AddToRoleAsync(_userManager.FindByNameAsync(user).Result, "admin").Result;
            }

            _context.SaveChanges();
        }
    }
}
