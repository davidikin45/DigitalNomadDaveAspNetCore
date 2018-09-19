using Microsoft.EntityFrameworkCore;

namespace DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF
{
    public interface IGenericRepositoryFactory
    {
        IGenericEFRepository<TEntity> Get<TEntity>(DbContext context) where TEntity : class;
        IGenericEFReadOnlyRepository<TEntity> GetReadOnly<TEntity>(DbContext context) where TEntity : class;
    }
}
