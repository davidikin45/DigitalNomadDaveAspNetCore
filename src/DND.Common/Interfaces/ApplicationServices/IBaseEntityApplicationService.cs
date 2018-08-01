using DND.Common.Implementation.DTOs;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Collections.Generic;
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

        TCreateDto GetCreateDefaultDto();

        object GetCreateDefaultCollectionItemDto(string collectionExpression);

        Result<TReadDto> Create(TCreateDto dto, string createdBy);

        Task<Result<TReadDto>> CreateAsync(TCreateDto dto, string createdBy, CancellationToken cancellationToken = default(CancellationToken));

        List<Result> BulkCreate(TCreateDto[] dtos, string createdBy);

        Task<List<Result>> BulkCreateAsync(TCreateDto[] dtos, string createdBy, CancellationToken cancellationToken = default(CancellationToken));

        Result Update(object id, TUpdateDto dto, string updatedBy);

        Task<Result> UpdateAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        List<Result> BulkUpdate(TUpdateDto[] dtos, string updatedBy);

        Task<List<Result>> BulkUpdateAsync(TUpdateDto[] dtos, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        Result UpdateGraph(object id, TUpdateDto dto, string updatedBy);

        Task<Result> UpdateGraphAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        List<Result> BulkUpdateGraph(TUpdateDto[] dtos, string updatedBy);

        Task<List<Result>> BulkUpdateGraphAsync(TUpdateDto[] dtos, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        Result UpdatePartial(object id, JsonPatchDocument dtoPatch, string updatedBy);

        List<Result> BulkUpdatePartial(PatchDto[] dtoPatches, string updatedBy);

        Task<Result> UpdatePartialAsync(object id, JsonPatchDocument dtoPatch, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<Result>> BulkUpdatePartialAsync(PatchDto[] dtoPatches, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(object id, string deletedBy);

        Task<Result> DeleteAsync(object id, string deletedBy, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(TDeleteDto dto, string deletedBy);

        Task<Result> DeleteAsync(TDeleteDto dto, string deletedBy, CancellationToken cancellationToken = default(CancellationToken));

        List<Result> BulkDelete(TDeleteDto[] dtos, string deletedBy);

        Task<List<Result>> BulkDeleteAsync(TDeleteDto[] dtos, string deletedBy, CancellationToken cancellationToken = default(CancellationToken));

        TUpdateDto GetUpdateDtoById(object id);

        IEnumerable<TUpdateDto> GetUpdateDtosByIds(IEnumerable<object> ids);

        Task<TUpdateDto> GetUpdateDtoByIdAsync(object id, CancellationToken cancellationToken);

        Task<IEnumerable<TUpdateDto>> GetUpdateDtosByIdsAsync(CancellationToken cancellationToken, IEnumerable<object> ids);

        TDeleteDto GetDeleteDtoById(object id);

        IEnumerable<TDeleteDto> GetDeleteDtosByIds(IEnumerable<object> ids);

        Task<TDeleteDto> GetDeleteDtoByIdAsync(object id, CancellationToken cancellationToken);

        Task<IEnumerable<TDeleteDto>> GetDeleteDtosByIdsAsync(CancellationToken cancellationToken, IEnumerable<object> ids);

        Result TriggerAction(object id, ActionDto action, string triggeredBy);

        Task<Result> TriggerActionAsync(object id, ActionDto action, string triggeredBy, CancellationToken cancellationToken = default(CancellationToken));

        List<Result> TriggerActions(BulkActionDto[] actions, string triggeredBy);

        Task<List<Result>> TriggerActionsAsync(BulkActionDto[] actions, string triggeredBy, CancellationToken cancellationToken = default(CancellationToken));
    }
}
