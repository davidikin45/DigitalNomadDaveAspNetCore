using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Solution.Base.Interfaces.Repository;

using Solution.Base.Implementation.Models;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;

namespace Solution.Base.Interfaces.UnitOfWork
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
