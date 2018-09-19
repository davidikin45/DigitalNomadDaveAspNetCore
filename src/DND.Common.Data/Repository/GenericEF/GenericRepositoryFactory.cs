using DND.Common.Infrastructure.Interfaces.Data.Repository.GenericEF;
using Microsoft.EntityFrameworkCore;

namespace DND.Common.Data.Repository.GenericEF
{
    public class GenericRepositoryFactory : IGenericRepositoryFactory
    {
        public virtual IGenericEFRepository<TEntity> Get<TEntity>(DbContext context) where TEntity : class
        {
            return new GenericEFRepository<TEntity>(context);
        }

        public virtual IGenericEFReadOnlyRepository<TEntity> GetReadOnly<TEntity>(DbContext context) where TEntity : class
        {
            return new GenericEFReadOnlyRepository<TEntity>(context);
        }
    }
}
