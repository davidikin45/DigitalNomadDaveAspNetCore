using AutoMapper;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Persistance;
using Solution.Base.Interfaces.Services;
using Solution.Base.Interfaces.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace Solution.Base.Implementation.Services
{
    public abstract class BaseEntityService<TContext, TEntity, TDto> : BaseEntityReadOnlyService<TContext, TEntity, TDto>, IBaseEntityService<TDto>
          where TContext : IBaseDbContext
          where TEntity : class, IBaseEntityAggregateRoot, IBaseEntityAuditable, new()
          where TDto : class, IBaseEntity

    {
        public BaseEntityService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
           : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public BaseEntityService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
           : base(baseUnitOfWorkScopeFactory)
        {

        }

        public virtual TDto Create(TDto dto)
        {
            var bo = Mapper.Map<TEntity>(dto);
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Create(bo, "");
                unitOfWork.Complete();

                return Mapper.Map<TDto>(bo);
            }
        }

        public virtual async Task<TDto> CreateAsync(TDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Create(bo, "");
                await unitOfWork.CompleteAsync(cancellationToken);

                return Mapper.Map<TDto>(bo);
            }
        }

        public virtual void Update(TDto dto)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                var persistedBO = unitOfWork.Repository<TContext, TEntity>().GetById(dto.Id);
                Mapper.Map(dto, persistedBO);

                unitOfWork.Repository<TContext, TEntity>().Update(persistedBO, "");
                unitOfWork.Complete();
            }
        }

        public virtual async Task UpdateAsync(TDto dto, CancellationToken cancellationToken)
        {
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var persistedBO = unitOfWork.Repository<TContext, TEntity>().GetById(dto.Id);
                Mapper.Map(dto, persistedBO);

                unitOfWork.Repository<TContext, TEntity>().Update(persistedBO, "");
                await unitOfWork.CompleteAsync(cancellationToken);
            }
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
            using (var unitOfWork = UnitOfWorkFactory.Create())
            {
                unitOfWork.Repository<TContext, TEntity>().Delete(bo);
                unitOfWork.Complete();
            }
        }

        public virtual async Task DeleteAsync(TDto dto, CancellationToken cancellationToken)
        {
            var bo = Mapper.Map<TEntity>(dto);
            using (var unitOfWork = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                unitOfWork.Repository<TContext, TEntity>().Delete(bo);
                await unitOfWork.CompleteAsync(cancellationToken);
            }
        }

    }
}
