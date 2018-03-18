using DND.Common.Interfaces.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.DomainServices
{
    public interface IBaseEntityDomainService<TEntity> : IBaseEntityReadOnlyDomainService<TEntity>
          where TEntity : class, IBaseEntity
    {
        TEntity Create(TEntity entity);

        Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        void Update(TEntity entity);

        Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(object id);

        Task DeleteAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));
    }
}
