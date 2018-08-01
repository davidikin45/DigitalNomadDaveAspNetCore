using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Data;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using DND.Common.Implementation.DTOs;
using DND.Common.Implementation.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class BaseEntityApplicationService<TEntity, TCreateDto, TReadDto, TUpdateDto, TDeleteDto, TDomainService> : BaseEntityReadOnlyApplicationService<TEntity, TReadDto, TDomainService>, IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, new()
          where TCreateDto : class, IBaseDto
          where TReadDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
          where TUpdateDto : class, IBaseDto, IBaseDtoConcurrencyAware
          where TDeleteDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
          where TDomainService : IBaseEntityDomainService<TEntity>
    {

        public BaseEntityApplicationService(TDomainService domainService, IMapper mapper)
           : base(domainService, mapper)
        {

        }

        public BaseEntityApplicationService(TDomainService domainService)
           : base(domainService)
        {

        }

        #region GetCreateDefaultDto
        public virtual TCreateDto GetCreateDefaultDto()
        {
            var bo = DomainService.GetNewEntityInstance();
            return Mapper.Map<TCreateDto>(bo);
        }
        #endregion

        #region Create

        public virtual Result<TReadDto> Create(TCreateDto dto, string createdBy)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var bo = Mapper.Map<TEntity>(dto);

            var result = DomainService.Create(bo, createdBy);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return Result.ObjectValidationFail<TReadDto>(result.ObjectValidationErrors);
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok(Mapper.Map<TReadDto>(bo));
        }

        public virtual async Task<Result<TReadDto>> CreateAsync(TCreateDto dto, string createdBy, CancellationToken cancellationToken)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var bo = Mapper.Map<TEntity>(dto);

            var result = await DomainService.CreateAsync(bo, createdBy).ConfigureAwait(false);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return Result.ObjectValidationFail<TReadDto>(result.ObjectValidationErrors);
                    case ErrorType.ObjectDoesNotExist:
                        return Result.ObjectDoesNotExist<TReadDto>();
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok(Mapper.Map<TReadDto>(bo));
        }
        #endregion

        #region Bulk Create
        public virtual List<Result> BulkCreate(TCreateDto[] dtos, string createdBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = Create(dto, createdBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkCreateAsync(TCreateDto[] dtos, string createdBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                var result = await CreateAsync(dto, createdBy, cancellationToken);
                results.Add(result);
            }
            return results;
        }
        #endregion

        #region GetUpdateDtoById
        public virtual TUpdateDto GetUpdateDtoById(object id)
        {
            var bo = DomainService.GetById(id, true);
            return Mapper.Map<TUpdateDto>(bo);
        }

        public virtual async Task<TUpdateDto> GetUpdateDtoByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bo = await DomainService.GetByIdAsync(id, cancellationToken, true);
            return Mapper.Map<TUpdateDto>(bo);
        }

        #endregion

        #region GetUpdateDtosByIds
        public virtual IEnumerable<TUpdateDto> GetUpdateDtosByIds(IEnumerable<object> ids)
        {
            var result = DomainService.GetByIds(ids);
            return Mapper.Map<IEnumerable<TUpdateDto>>(result);
        }

        public virtual async Task<IEnumerable<TUpdateDto>> GetUpdateDtosByIdsAsync(CancellationToken cancellationToken,
       IEnumerable<object> ids)
        {
            var result = await DomainService.GetByIdsAsync(cancellationToken, ids).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TUpdateDto>>(result);
        }
        #endregion

        #region Update

        public virtual Result Update(object id, TUpdateDto dto, string updatedBy)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail(objectValidationErrors);
            }

            var persistedBO = DomainService.GetById(id);

            Mapper.Map(dto, persistedBO);

            var result = DomainService.Update(persistedBO, updatedBy);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public virtual Result UpdateGraph(object id, TUpdateDto dto, string updatedBy)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail(objectValidationErrors);
            }

            var persistedBO = DomainService.GetById(id, true);

            Mapper.Map(dto, persistedBO);

            var result = DomainService.UpdateGraph(persistedBO, updatedBy);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail(objectValidationErrors);
            }

            var persistedBO = await DomainService.GetByIdAsync(id, cancellationToken);

            Mapper.Map(dto, persistedBO);

            var result = await DomainService.UpdateAsync(persistedBO, updatedBy, cancellationToken).ConfigureAwait(false);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateGraphAsync(object id, TUpdateDto dto, string updatedBy, CancellationToken cancellationToken)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail(objectValidationErrors);
            }

            var persistedBO = await DomainService.GetByIdAsync(id, cancellationToken, true);

            Mapper.Map(dto, persistedBO);

            var result = await DomainService.UpdateGraphAsync(persistedBO, updatedBy, cancellationToken).ConfigureAwait(false);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }
        #endregion

        #region Bulk Update
        public virtual List<Result> BulkUpdate(TUpdateDto[] dtos, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = Update(((IBaseDtoWithId)dto).Id, dto, updatedBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }

            }
            return results;
        }

        public virtual List<Result> BulkUpdateGraph(TUpdateDto[] dtos, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = UpdateGraph(((IBaseDtoWithId)dto).Id, dto, updatedBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkUpdateAsync(TUpdateDto[] dtos, string updatedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = await UpdateAsync(((IBaseDtoWithId)dto).Id, dto, updatedBy, cancellationToken);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkUpdateGraphAsync(TUpdateDto[] dtos, string updatedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = await UpdateGraphAsync(((IBaseDtoWithId)dto).Id, dto, updatedBy, cancellationToken);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }
        #endregion

        #region Update Partial

        public virtual Result UpdatePartial(object id, JsonPatchDocument dtoPatch, string updatedBy)
        {
            var dto = GetUpdateDtoById(id);

            if (dto == null)
            {
                return Result.ObjectDoesNotExist();
            }

            var ops = new List<Operation<TUpdateDto>>();
            foreach (var op in dtoPatch.Operations)
            {
                ops.Add(new Operation<TUpdateDto>(op.op, op.path, op.from, op.value));
            }

            var dtoPatchTypes = new JsonPatchDocument<TUpdateDto>(ops, dtoPatch.ContractResolver);

            dtoPatchTypes.ApplyTo(dto);

            var result = UpdateGraph(id, dto, updatedBy);

            return Result.Ok();
        }

        public virtual async Task<Result> UpdatePartialAsync(object id, JsonPatchDocument dtoPatch, string updatedBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var dto = await GetUpdateDtoByIdAsync(id, cancellationToken);

            if (dto == null)
            {
                return Result.ObjectDoesNotExist();
            }

            var ops = new List<Operation<TUpdateDto>>();
            foreach (var op in dtoPatch.Operations)
            {
                ops.Add(new Operation<TUpdateDto>(op.op, op.path, op.from, op.value));
            }

            var dtoPatchTypes = new JsonPatchDocument<TUpdateDto>(ops, dtoPatch.ContractResolver);

            dtoPatchTypes.ApplyTo(dto);

            var result = await UpdateGraphAsync(id, dto, updatedBy, cancellationToken);

            return Result.Ok();
        }
        #endregion

        #region Bulk Partial Update
        public virtual List<Result> BulkUpdatePartial(PatchDto[] dtoPatches, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtoPatches)
            {
                try
                {
                    var result = UpdatePartial(dto.Id, dto.Commands, updatedBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public virtual async Task<List<Result>> BulkUpdatePartialAsync(PatchDto[] dtoPatches, string updatedBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var results = new List<Result>();
            foreach (var dto in dtoPatches)
            {
                try
                {
                    var result = await UpdatePartialAsync(dto.Id, dto.Commands, updatedBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }
        #endregion

        #region GetDeleteDtoById
        public virtual TDeleteDto GetDeleteDtoById(object id)
        {
            var bo = DomainService.GetById(id);
            return Mapper.Map<TDeleteDto>(bo);
        }

        public virtual async Task<TDeleteDto> GetDeleteDtoByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bo = await DomainService.GetByIdAsync(id, cancellationToken);
            return Mapper.Map<TDeleteDto>(bo);
        }
        #endregion

        #region GetDeleteDtosByIds
        public virtual IEnumerable<TDeleteDto> GetDeleteDtosByIds(IEnumerable<object> ids)
        {
            var result = DomainService.GetByIds(ids);
            return Mapper.Map<IEnumerable<TDeleteDto>>(result);
        }

        public virtual async Task<IEnumerable<TDeleteDto>> GetDeleteDtosByIdsAsync(CancellationToken cancellationToken,
       IEnumerable<object> ids)
        {
            var result = await DomainService.GetByIdsAsync(cancellationToken, ids).ConfigureAwait(false);
            return Mapper.Map<IEnumerable<TDeleteDto>>(result);
        }
        #endregion

        #region Delete

        public virtual Result Delete(object id, string deletedBy)
        {
            TDeleteDto deleteDto = GetDeleteDtoById(id);
            return Delete(deleteDto, deletedBy);
        }

        public virtual async Task<Result> DeleteAsync(object id, string deletedBy, CancellationToken cancellationToken)
        {
            TDeleteDto deleteDto = await GetDeleteDtoByIdAsync(id, cancellationToken);
            return await DeleteAsync(deleteDto, deletedBy, cancellationToken).ConfigureAwait(false);
        }

        public virtual Result Delete(TDeleteDto dto, string deletedBy)
        {
            var bo = Mapper.Map<TEntity>(dto);
            var result = DomainService.Delete(bo, deletedBy);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(TDeleteDto dto, string deletedBy, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);
            var result = await DomainService.DeleteAsync(bo, deletedBy, cancellationToken).ConfigureAwait(false);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        #endregion

        #region Bulk Delete
        public virtual List<Result> BulkDelete(TDeleteDto[] dtos, string deletedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = Delete(dto, deletedBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> BulkDeleteAsync(TDeleteDto[] dtos, string deletedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                try
                {
                    var result = await DeleteAsync(dto, deletedBy, cancellationToken);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }
        #endregion

        #region GetCreateDefaultCollectionItemDto
        public virtual object GetCreateDefaultCollectionItemDto(string collectionExpression)
        {
            var type = RelationshipHelper.GetCollectionExpressionCreateType(collectionExpression, typeof(TUpdateDto));
            return Activator.CreateInstance(type);
        }
        #endregion

        #region TriggerActions

        public virtual Result TriggerAction(object id, ActionDto action, string triggeredBy)
        {
            var result = DomainService.TriggerAction(id, action.Action, triggeredBy);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public async virtual Task<Result> TriggerActionAsync(object id, ActionDto action, string triggeredBy, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await DomainService.TriggerActionAsync(id, action.Action, triggeredBy, cancellationToken).ConfigureAwait(false);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return result;
                    case ErrorType.ObjectDoesNotExist:
                        return result;
                    case ErrorType.ConcurrencyConflict:
                        return result;
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }
        #endregion

        #region Bulk Trigger Actions
        public virtual List<Result> TriggerActions(BulkActionDto[] actions, string triggeredBy)
        {
            var results = new List<Result>();
            foreach (var action in actions)
            {
                try
                {
                    var result = TriggerAction(action.Id, action, triggeredBy);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        public async virtual Task<List<Result>> TriggerActionsAsync(BulkActionDto[] actions, string triggeredBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var action in actions)
            {
                try
                {
                    var result = await TriggerActionAsync(action.Id, action, triggeredBy, cancellationToken);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    results.Add(Result.Fail(ErrorType.UnknownError));
                }
            }
            return results;
        }

        #endregion
    }
}
