using DND.Common.Infrastructure.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF
{
    public interface IGenericEFRepository<TEntity> : IGenericEFReadOnlyRepository<TEntity>
        where TEntity : class
    {
        Result InsertOrUpdate(TEntity entity, string createdUpdateBy = null);

        Task<Result> InsertOrUpdateAsync(CancellationToken cancellationToken, TEntity entity, string createdUpdateBy = null);

        Result Insert(TEntity entity, string createdBy = null);

        Task<Result> InsertAsync(CancellationToken cancellationToken, TEntity entity, string createdBy = null);

        Result Update(TEntity entity, string modifiedBy = null);

        Task<Result> UpdateAsync(CancellationToken cancellationToken, TEntity entity, string modifiedBy = null);

        //Update Graph is specifically for Disconnected. It should take care of insert, update, delete of child collections.
        Result UpdateGraph(TEntity entity, string modifiedBy = null);

        //Update Graph is specifically for Disconnected. It should take care of insert, update, delete of child collections.
        Task<Result> UpdateGraphAsync(CancellationToken cancellationToken, TEntity entity, string modifiedBy = null);

        Result Delete(object id, string deletedBy = null);

        Task<Result> DeleteAsync(CancellationToken cancellationToken, object id, string deletedBy = null);

        Result Delete(TEntity entity, string deletedBy = null);

        Task<Result> DeleteAsync(CancellationToken cancellationToken, TEntity entity, string deletedBy = null);
    }
}
