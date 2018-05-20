using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.ApplicationServices
{
    public interface IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto> : IBaseEntityReadOnlyApplicationService<TReadDto>
          where TCreateDto : class, IBaseDto
          where TReadDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
          where TUpdateDto : class, IBaseDto, IBaseDtoConcurrencyAware
          where TDeleteDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
    {
        Result<TReadDto> Create(TCreateDto dto, string createdBy);

        Task<Result<TReadDto>> CreateAsync(TCreateDto dto, string createdBy, CancellationToken cancellationToken = default(CancellationToken));

        Result Update(object id, TUpdateDto dto, string updatedBy);

        Task<Result> UpdateAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(object id);

        Task<Result> DeleteAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(TDeleteDto dto);

        Task<Result> DeleteAsync(TDeleteDto dto, CancellationToken cancellationToken = default(CancellationToken));

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
