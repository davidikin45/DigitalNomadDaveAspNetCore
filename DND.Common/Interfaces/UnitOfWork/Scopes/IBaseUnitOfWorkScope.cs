using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DND.Common.Interfaces.Repository;

using DND.Common.Implementation.Models;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;

namespace DND.Common.Interfaces.UnitOfWork
{
    public interface IBaseUnitOfWorkScope
    {
        IBaseRepository<TEntity> Repository<TContext, TEntity>()
              where TContext : IBaseDbContext
             where TEntity : class, IBaseEntity, IBaseEntityAuditable, new();

        IBaseReadOnlyRepository<TEntity> ReadOnlyRepository<TContext, TEntity>()
            where TContext : IBaseDbContext
           where TEntity : class, IBaseEntity, IBaseEntityAuditable, new();
    }
}
