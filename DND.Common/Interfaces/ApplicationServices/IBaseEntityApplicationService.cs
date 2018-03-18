using DND.Common.Interfaces.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.ApplicationServices
{
    public interface IBaseEntityApplicationService<TDto> : IBaseEntityReadOnlyApplicationService<TDto>
          where TDto : class, IBaseEntity
    {
        TDto Create(TDto dto);

        Task<TDto> CreateAsync(TDto dto, CancellationToken cancellationToken = default(CancellationToken));

        void Update(TDto dto);

        Task UpdateAsync(TDto dto, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(object id);

        Task DeleteAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(TDto dto);

        Task DeleteAsync(TDto dto, CancellationToken cancellationToken = default(CancellationToken));
    }
}
