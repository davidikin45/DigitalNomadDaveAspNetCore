using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data;
using DND.Domain.Blog.Locations;
using DND.Interfaces.Blog.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.Locations.Services
{
    public class LocationDomainService : DomainServiceEntityBase<ApplicationContext, Location>, ILocationDomainService
    {
        public LocationDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override async Task<Result<Location>> CreateAsync(CancellationToken cancellationToken, Location entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(cancellationToken, entity, createdBy).ConfigureAwait(false);
        }

        public async Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, Location>().GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(urlSlug)).ConfigureAwait(false);
            }
        }

        public override async Task<Result> UpdateAsync(CancellationToken cancellationToken, DND.Domain.Blog.Locations.Location entity, string updatedBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

           return await base.UpdateAsync(cancellationToken, entity, updatedBy).ConfigureAwait(false);
        }
    }
}