using DND.Common.Enums;
using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Locations;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.Locations.Services
{
    public class LocationDomainService : BaseEntityDomainService<IApplicationDbContext, Location>, ILocationDomainService
    {
        public LocationDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override async Task<Result<Location>> CreateAsync(Location entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, Location>().GetFirstAsync(t => t.UrlSlug.Equals(urlSlug)).ConfigureAwait(false);
            }
        }

        public override async Task<Result> UpdateAsync(DND.Domain.Blog.Locations.Location entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

           return await base.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        }

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(Location entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }


    }
}