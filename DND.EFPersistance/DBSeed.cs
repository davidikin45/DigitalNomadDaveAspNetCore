using DND.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DND.EFPersistance
{
    public static class DBSeed
    {
        public static void Seed(ApplicationDbContext context)
        {
            AddRoles(context);
            AddUsers(context);
            context.SaveChanges();
        }

        private static void AddRoles(ApplicationDbContext context)
        {
           
        }

        private static void AddUsers(ApplicationDbContext context)
        {
           
        }

    }
}
