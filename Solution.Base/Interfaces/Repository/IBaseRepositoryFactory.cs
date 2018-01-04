using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;
using System.Threading;

namespace Solution.Base.Interfaces.Repository
{
    public interface IBaseRepositoryFactory
    {
        IBaseRepository<TEntity> Get<TEntity>(IBaseDbContext context, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, IBaseEntityAuditable, new();
        IBaseReadOnlyRepository<TEntity> GetReadOnly<TEntity>(IBaseDbContext context, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, new();
    }
}
