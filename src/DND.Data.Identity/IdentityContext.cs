using DND.Common.Data;
using DND.Data.Identity.Configurations;
using DND.Domain.Identity.Users;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace DND.Data.Identity
{
    //Set Context Project as Visual Studio StartUp Project
    //Add-Migration InitialMigration -Context IdentityContext
    //Add-Migration "Name" -Context ApplicationIdentityDbContext
    //Update-Database
    //Remove-Migration
    public class IdentityContext : IdentityDbContextBase<User>
    { 
        public IdentityContext(DbContextOptions options)
            :base(options)
        {
           var framework = RuntimeInformation.FrameworkDescription;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserConfiguration());
            builder.Entity<User>().ToTable("User");
        }

        public override void Seed()
        {
            DbSeed.Seed(this);
        }
    }
}
