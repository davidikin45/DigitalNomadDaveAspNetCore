using AutoMapper;
using Solution.Base.Interfaces.ApplicationServices;
using Solution.Base.Interfaces.DomainServices;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.ApplicationServices
{
    public abstract class BaseEntityApplicationService<TContext, TEntity, TDto> : BaseEntityReadOnlyApplicationService<TContext, TEntity, TDto>, IBaseEntityApplicationService<TDto>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, new()
          where TDto : class, IBaseEntity

    {
        protected virtual IBaseEntityDomainService<TEntity> DomainService { get; }

        public BaseEntityApplicationService(IBaseEntityDomainService<TEntity> domainService, IMapper mapper)
           : base(domainService, mapper)
        {
            DomainService = domainService;
        }

        public BaseEntityApplicationService(IBaseEntityDomainService<TEntity> domainService)
           : base(domainService)
        {
            DomainService = domainService;
        }

        public virtual TDto Create(TDto dto)
        {
            var bo = Mapper.Map<TEntity>(dto);

            DomainService.Create(bo);

            return Mapper.Map<TDto>(bo);           
        }

        public virtual async Task<TDto> CreateAsync(TDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);

            await DomainService.CreateAsync(bo);

            return Mapper.Map<TDto>(bo);
        }

        public virtual void Update(TDto dto)
        {
            var persistedBO = DomainService.GetById(dto.Id);

            Mapper.Map(dto, persistedBO);

            DomainService.Update(persistedBO);
        }

        public virtual async Task UpdateAsync(TDto dto, CancellationToken cancellationToken)
        {
            var persistedBO = await DomainService.GetByIdAsync(dto.Id, cancellationToken);

            Mapper.Map(dto, persistedBO);

            await DomainService.UpdateAsync(persistedBO, cancellationToken);
        }

        public virtual void Delete(object id)
        {
            TDto entity = GetById(id);
            Delete(entity);
        }

        public virtual async Task DeleteAsync(object id, CancellationToken cancellationToken)
        {
            TDto entity = await GetByIdAsync(id,cancellationToken);
            await DeleteAsync(entity, cancellationToken);
        }

        public virtual void Delete(TDto dto)
        {
            var bo = Mapper.Map<TEntity>(dto);
            DomainService.Delete(bo);
        }

        public virtual async Task DeleteAsync(TDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);
            await DomainService.DeleteAsync(bo, cancellationToken);
        }
    }
}
