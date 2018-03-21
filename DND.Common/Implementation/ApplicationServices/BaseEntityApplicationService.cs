using AutoMapper;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Persistance;
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

        public virtual TReadDto Create(TCreateDto dto)
        {
            var bo = Mapper.Map<TEntity>(dto);

            DomainService.Create(bo);

            return Mapper.Map<TReadDto>(bo);           
        }

        public virtual async Task<TReadDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);

            await DomainService.CreateAsync(bo);

            return Mapper.Map<TReadDto>(bo);
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

        public virtual void Update(object id, TUpdateDto dto)
        {
            var persistedBO = DomainService.GetById(id);

            Mapper.Map(dto, persistedBO);

            DomainService.Update(persistedBO);
        }

        public virtual async Task UpdateAsync(object id, TUpdateDto dto, CancellationToken cancellationToken)
        {
            var persistedBO = await DomainService.GetByIdAsync(id, cancellationToken);

            Mapper.Map(dto, persistedBO);

            await DomainService.UpdateAsync(persistedBO, cancellationToken);
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

        public virtual void Delete(object id)
        {
            TDeleteDto deleteDto = GetDeleteDtoById(id);
            Delete(deleteDto);
        }

        public virtual async Task DeleteAsync(object id, CancellationToken cancellationToken)
        {
            TDeleteDto deleteDto = await GetDeleteDtoByIdAsync(id, cancellationToken);
            await DeleteAsync(deleteDto, cancellationToken);
        }

        public virtual void Delete(TDeleteDto dto)
        {
            var bo = Mapper.Map<TEntity>(dto);
            DomainService.Delete(bo);
        }

        public virtual async Task DeleteAsync(TDeleteDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);
            await DomainService.DeleteAsync(bo, cancellationToken);
        }
    }
}
