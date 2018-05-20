using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Domain.Blog.Authors;
using DND.Common.Implementation.Validation;
using DND.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace DND.DomainServices.Authors.Services
{
    public class AuthorDomainService : BaseEntityDomainService<IApplicationDbContext, Author>, IAuthorDomainService
    {
        public AuthorDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, Author>().GetFirstAsync(c => c.UrlSlug.Equals(authorSlug));
            }
        }

        public async override Task<Result> UpdateAsync(Author entity, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.UpdateAsync(entity, updatedBy, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<Result<Author>> CreateAsync(Author entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, createdBy, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(Author entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}