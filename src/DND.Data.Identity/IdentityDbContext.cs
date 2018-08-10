using DND.Common.DomainEvents;
using DND.Common.Implementation.Data;
using DND.Data.Identity.Configurations;
using DND.Domain.Identity.Users;
using Microsoft.EntityFrameworkCore;

namespace DND.Data.Identity
{
    //Set Context Project as Visual Studio StartUp Project
    //Add-Migration InitialMigration -Context ApplicationIdentityDbContext
    //Add-Migration "Name" -Context ApplicationIdentityDbContext
    //Update-Database
    //Remove-Migration
    public class IdentityDbContext : BaseIdentityDbContext<User>
    { 
        public IdentityDbContext(DbContextOptions options, IDomainEvents domainEvents = null)
            :base(options, domainEvents)
        {
            Database.SetCommandTimeout(180);
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
