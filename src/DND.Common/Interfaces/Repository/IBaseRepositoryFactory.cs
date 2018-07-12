using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using System.Threading;
using DND.Common.Interfaces.UnitOfWork;

namespace DND.Common.Interfaces.Repository
{
    public interface IGenericRepositoryFactory
    {
        IGenericEFRepository<TEntity> Get<TEntity>(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, IBaseEntityAuditable, new();
        IGenericEFReadOnlyRepository<TEntity> GetReadOnly<TEntity>(IBaseDbContext context, IBaseUnitOfWorkScope uow, CancellationToken cancellationToken = default(CancellationToken)) where TEntity : class, IBaseEntity, new();
    }
}
