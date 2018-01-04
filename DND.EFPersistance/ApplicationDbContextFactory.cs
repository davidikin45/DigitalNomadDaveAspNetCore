using DND.Domain;
using Solution.Base.Implementation.Persistance;
using System.Data.Entity.Infrastructure;

namespace DND.EFPersistance
{
    public class ApplicationDbContextFactory : IDbContextFactory<ApplicationDbContext>
    {
        ApplicationDbContext IDbContextFactory<ApplicationDbContext>.Create()
        {
            return new ApplicationDbContext(DNDConnectionStrings.GetConnectionString("DefaultConnectionString"));
        }
    }
}
