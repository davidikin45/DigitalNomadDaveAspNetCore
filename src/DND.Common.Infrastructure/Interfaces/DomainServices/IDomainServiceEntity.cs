using DND.Common.Infrastructure.Validation;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Infrastructure.Interfaces.DomainServices
{
    public interface IDomainServiceEntity<TEntity> : IDomainServiceEntityReadOnly<TEntity>
          where TEntity : class
    {
        TEntity GetNewEntityInstance();

        Result<TEntity> Create(TEntity entity, string createdBy);

        Task<Result<TEntity>> CreateAsync(CancellationToken cancellationToken, TEntity entity, string createdBy);

        Result Update(TEntity entity, string updatedBy);

        Task<Result> UpdateAsync(CancellationToken cancellationToken, TEntity entity, string updatedBy);

        Result UpdateGraph(TEntity entity, string updatedBy);

        Task<Result> UpdateGraphAsync(CancellationToken cancellationToken, TEntity entity, string updatedBy);

        Result Delete(object id, string deletedBy);

        Task<Result> DeleteAsync(CancellationToken cancellationToken, object id, string deletedBy);

        Result Delete(TEntity entity, string deletedBy);

        Task<Result> DeleteAsync(CancellationToken cancellationToken, TEntity entity, string deletedBy);

        Result TriggerAction(object id, string action, string triggeredBy);

        Task<Result> TriggerActionAsync(object id, string action, string triggeredBy, CancellationToken cancellationToken = default(CancellationToken));
    }
}
