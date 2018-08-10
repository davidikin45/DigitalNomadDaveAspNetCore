using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.Data
{
    public interface IDbContextFactoryProducerSingleton
    {
        IDbContextFactory<TIDbContext> GetFactory<TIDbContext>() where TIDbContext : IBaseDbContext;
        IDbContextAbstractFactory GetAbstractFactory<TIDbContext>() where TIDbContext : IBaseDbContext;
    }
}
