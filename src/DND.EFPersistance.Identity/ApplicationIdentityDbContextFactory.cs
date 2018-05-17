using DND.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.EFPersistance.Identity
{
    public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();
            builder.UseSqlServer(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"));

            //EF Core Doesn't support Ambient Transactions
            builder.ConfigureWarnings(x => x.Ignore(RelationalEventId.AmbientTransactionWarning));
            return new ApplicationIdentityDbContext(builder.Options);
        }
    }
}
