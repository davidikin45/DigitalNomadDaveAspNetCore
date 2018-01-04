using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solution.Base.Interfaces.Models;

namespace Solution.Base.Interfaces.Repository
{
    public interface IBaseRepository<TEntity> : IBaseReadOnlyRepository<TEntity>
         where TEntity : class, IBaseEntity, IBaseEntityAuditable, new()

    {
        void CreateOrUpdate(TEntity entity, string createdUpdateBy = null);

        void Create(TEntity entity, string createdBy = null);

        void Update(TEntity entity, string modifiedBy = null);

        void Delete(object id);

        void Delete(TEntity entity);
    }

}
