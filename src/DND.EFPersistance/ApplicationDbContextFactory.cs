using DND.Domain;
using DND.Common.Implementation.Persistance;
using System.Data.Entity.Infrastructure;
using System.Data.SQLite;

namespace DND.EFPersistance
{
    public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        ApplicationDbContext IDbContextFactory<ApplicationDbContext>.Create()
        {
            if (bool.Parse(ConnectionStrings.GetConnectionString("UseSQLite")))
            {
                var con = new SQLiteConnection()
                {
                    ConnectionString = ConnectionStrings.GetConnectionString("SQLite")
                };
                return new ApplicationDbContext(con);
            }
            else
            {
                return new ApplicationDbContext(ConnectionStrings.GetConnectionString("DefaultConnectionString"));
            }
        }
    }
}
