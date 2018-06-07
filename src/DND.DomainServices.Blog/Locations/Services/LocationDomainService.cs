using DND.Common.Implementation.DomainServices;
using DND.Common.Implementation.Validation;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Blog.Locations;
using DND.Interfaces.Blog.Data;
using DND.Interfaces.Blog.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.Locations.Services
{
    public class LocationDomainService : BaseEntityDomainService<IBlogDbContext, Location>, ILocationDomainService
    {
        public LocationDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override async Task<Result<Location>> CreateAsync(Location entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, createdBy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IBlogDbContext, Location>().GetFirstAsync(t => t.UrlSlug.Equals(urlSlug)).ConfigureAwait(false);
            }
        }

        public override async Task<Result> UpdateAsync(DND.Domain.Blog.Locations.Location entity, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

           return await base.UpdateAsync(entity, updatedBy, cancellationToken).ConfigureAwait(false);
        }
    }
}