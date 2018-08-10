using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Data
{
    public interface IDbContextFactory<TIDbContext> : IDbContextAbstractFactory where TIDbContext : IBaseDbContext
    {
        TIDbContext CreateDbContext();
    }

    public interface IDbContextAbstractFactory
    {
        IBaseDbContext CreateBaseDbContext();
    }
}
