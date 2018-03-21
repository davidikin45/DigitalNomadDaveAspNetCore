using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.ApplicationServices
{
    public interface IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : IBaseEntityReadOnlyApplicationService<TReadDto>
          where TCreateDto : class, IBaseDto
          where TReadDto : class, IBaseDtoWithId
          where TUpdateDto : class, IBaseDto
          where TDeleteDto : class, IBaseDtoWithId
    {
        TReadDto Create(TCreateDto dto);

        Task<TReadDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default(CancellationToken));

        void Update(object id, TUpdateDto dto);

        Task UpdateAsync(object id, TUpdateDto dto, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(object id);

        Task DeleteAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        void Delete(TDeleteDto dto);

        Task DeleteAsync(TDeleteDto dto, CancellationToken cancellationToken = default(CancellationToken));

        TUpdateDto GetUpdateDtoById(object id)
           ;

        Task<TUpdateDto> GetUpdateDtoByIdAsync(object id,
             CancellationToken cancellationToken)
            ;

        TDeleteDto GetDeleteDtoById(object id)
           ;

        Task<TDeleteDto> GetDeleteDtoByIdAsync(object id,
             CancellationToken cancellationToken)
            ;
    }
}
