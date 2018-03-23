using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Implementation.ApplicationServices
{
    public abstract class BaseEntityApplicationService<TContext, TEntity, TCreateDto, TReadDto, TUpdateDto, TDeleteDto, TDomainService> : BaseEntityReadOnlyApplicationService<TContext, TEntity, TReadDto, TDomainService>, IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, new()
          where TCreateDto : class, IBaseDto
          where TReadDto : class, IBaseDtoWithId
          where TUpdateDto : class, IBaseDto
          where TDeleteDto : class, IBaseDtoWithId
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

        public virtual Result<TReadDto> Create(TCreateDto dto)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var bo = Mapper.Map<TEntity>(dto);

            var result = DomainService.Create(bo);
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

        public virtual async Task<Result<TReadDto>> CreateAsync(TCreateDto dto, CancellationToken cancellationToken)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail<TReadDto>(objectValidationErrors);
            }

            var bo = Mapper.Map<TEntity>(dto);

            var result = await DomainService.CreateAsync(bo);
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

        public virtual Result Update(object id, TUpdateDto dto)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail(objectValidationErrors);
            }

            var persistedBO = DomainService.GetById(id);

            Mapper.Map(dto, persistedBO);

            var result = DomainService.Update(persistedBO);
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

            return Result.Ok();
        }

        public virtual async Task<Result> UpdateAsync(object id, TUpdateDto dto, CancellationToken cancellationToken)
        {
            var objectValidationErrors = dto.Validate().ToList();
            if (objectValidationErrors.Any())
            {
                return Result.ObjectValidationFail(objectValidationErrors);
            }

            var persistedBO = await DomainService.GetByIdAsync(id, cancellationToken);

            Mapper.Map(dto, persistedBO);

            var result = await DomainService.UpdateAsync(persistedBO, cancellationToken);
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

        public virtual Result Delete(object id)
        {
            TDeleteDto deleteDto = GetDeleteDtoById(id);
            return Delete(deleteDto);
        }

        public virtual async Task<Result> DeleteAsync(object id, CancellationToken cancellationToken)
        {
            TDeleteDto deleteDto = await GetDeleteDtoByIdAsync(id, cancellationToken);
            return await DeleteAsync(deleteDto, cancellationToken);
        }

        public virtual Result Delete(TDeleteDto dto)
        {
            var bo = Mapper.Map<TEntity>(dto);
            var result = DomainService.Delete(bo);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return Result.ObjectValidationFail(result.ObjectValidationErrors);
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }

        public virtual async Task<Result> DeleteAsync(TDeleteDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);
            var result = await DomainService.DeleteAsync(bo, cancellationToken);
            if (result.IsFailure)
            {
                switch (result.ErrorType)
                {
                    case ErrorType.ObjectValidationFailed:
                        return Result.ObjectValidationFail(result.ObjectValidationErrors);
                    default:
                        throw new ArgumentException();
                }
            }

            return Result.Ok();
        }
    }
}
