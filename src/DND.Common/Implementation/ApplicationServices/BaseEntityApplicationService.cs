﻿using AutoMapper;
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

        public virtual TUpdateDto GetUpdateDtoById(object id)
        {
            var bo = DomainService.GetById(id);
            return Mapper.Map<TUpdateDto>(bo);
        }

        public virtual async Task<TUpdateDto> GetUpdateDtoByIdAsync(object id,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bo = await DomainService.GetByIdAsync(id, cancellationToken);
            return Mapper.Map<TUpdateDto>(bo);
        }

        public virtual List<Result> BulkUpdate(TUpdateDto[] dtos, string updatedBy)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                var result = Update(((IBaseDtoWithId)dto).Id, dto, updatedBy);
                results.Add(result);
            }
            return results;
        }

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

        public async virtual Task<List<Result>> BulkUpdateAsync(TUpdateDto[] dtos, string updatedBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var dto in dtos)
            {
                var result = await UpdateAsync(((IBaseDtoWithId)dto).Id, dto, updatedBy, cancellationToken);
                results.Add(result);
            }
            return results;
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

        public virtual List<Result> TriggerActions(BulkActionDto[] actions, string triggeredBy)
        {
            var results = new List<Result>();
            foreach (var action in actions)
            {
                var result = TriggerAction(action.Id, action, triggeredBy);
                results.Add(result);
            }
            return results;
        }

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

        public async virtual Task<List<Result>> TriggerActionsAsync(BulkActionDto[] actions, string triggeredBy, CancellationToken cancellationToken)
        {
            var results = new List<Result>();
            foreach (var action in actions)
            {
                var result = await TriggerActionAsync(action.Id, action, triggeredBy, cancellationToken);
                results.Add(result);
            }
            return results;
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
    }
}
