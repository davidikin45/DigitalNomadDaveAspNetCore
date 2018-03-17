using DND.Domain.DTOs;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class LocationDomainService : BaseEntityDomainService<IApplicationDbContext, Location>, ILocationDomainService
    {
        public LocationDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override Task<Location> CreateAsync(Location entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.CreateAsync(entity, cancellationToken);
        }

        public async Task<Location> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Location>().GetFirstAsync(t => t.UrlSlug.Equals(urlSlug));
            }
        }

        public override Task UpdateAsync(Location entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return base.UpdateAsync(entity, cancellationToken);
        }


    }
}