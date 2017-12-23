using DND.Domain.Models;
using Microsoft.EntityFrameworkCore;
using DND.Base.Extensions;
using DND.Base.Implementation.Persistance;
using System;
using System.Collections.Generic;
using System.Text;

namespace DND.EFPersistance
{
    public class ApplicationDbContext : BaseIdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options)
          : base(options)
        {
            this.Database.SetCommandTimeout(180);
        }

        public override void Initialize()
        {
            if(!this.AllMigrationsApplied())
            {
                Database.Migrate();
                DBSeed.Seed(this);
            }
        }
    }
}
