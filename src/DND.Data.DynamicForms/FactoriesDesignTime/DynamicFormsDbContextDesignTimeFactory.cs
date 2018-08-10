using DND.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Data.DynamicForms.FactoriesDesignTime
{
    public class DynamicFormsDbContextDesignTimeFactory : IDbContextFactory<DynamicFormsDbContext>
    {
        DynamicFormsDbContext IDbContextFactory<DynamicFormsDbContext>.Create()
        {
            if (bool.Parse(DNDConnectionStrings.GetConnectionString("UseSQLite")))
            {
                var con = new SQLiteConnection()
                {
                    ConnectionString = DNDConnectionStrings.GetConnectionString("SQLite")
                };
                return new DynamicFormsDbContext(con);
            }
            else
            {
                return new DynamicFormsDbContext(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"), false);
            }
        }
    }
}
