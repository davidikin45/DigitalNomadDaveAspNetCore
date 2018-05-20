﻿using DND.Common.Enums;
using DND.Common.Implementation.Validation;
using DND.Common.Interfaces.Models;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Common.Interfaces.DomainServices
{
    public interface IBaseEntityDomainService<TEntity> : IBaseEntityReadOnlyDomainService<TEntity>
          where TEntity : class, IBaseEntity
    {
        Result<TEntity> Create(TEntity entity, string createdBy);

        Task<Result<TEntity>> CreateAsync(TEntity entity, string createdBy,  CancellationToken cancellationToken = default(CancellationToken));

        Result Update(TEntity entity, string updatedBy);

        Task<Result> UpdateAsync(TEntity entity, string updatedBy, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(object id);

        Task<Result> DeleteAsync(object id, CancellationToken cancellationToken = default(CancellationToken));

        Result Delete(TEntity entity);

        Task<Result> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        Result Validate(TEntity entity, ValidationMode mode);

        Task<Result> ValidateAsync(TEntity entity, ValidationMode mode);
    }
}
