using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;

namespace DND.Common.Interfaces.Repository
{
    public interface IGenericEFRepository<TEntity> : IGenericEFReadOnlyRepository<TEntity>
         where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()

    {
        Result InsertOrUpdate(TEntity entity, string createdUpdateBy = null);

        Task<Result> InsertOrUpdateAsync(TEntity entity, string createdUpdateBy = null, CancellationToken cancellationToken = default(CancellationToken));

        Result Insert(TEntity entity, string createdBy = null);

        Task<Result> InsertAsync(TEntity entity, string createdBy = null, CancellationToken cancellationToken = default(CancellationToken));

        Result Update(TEntity entity, string modifiedBy = null);

        Task<Result> UpdateAsync(TEntity entity, string modifiedBy = null, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(object id, string deletedBy = null);

        Task<Result> DeleteAsync(object id, string deletedBy = null, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(TEntity entity, string deletedBy = null);

        Task<Result> DeleteAsync(TEntity entity, string deletedBy = null, CancellationToken cancellationToken = default(CancellationToken));
    }

}
