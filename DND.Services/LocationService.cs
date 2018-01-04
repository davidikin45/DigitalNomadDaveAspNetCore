using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Extensions;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.UnitOfWork;
using System.Threading;
using System.Threading.Tasks;
using Solution.Base.Infrastructure;

namespace DND.Services
{
    public class LocationService : BaseEntityService<IApplicationDbContext, Location, LocationDTO>, ILocationService
    {
        public LocationService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public override Task<LocationDTO> CreateAsync(LocationDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.CreateAsync(dto, cancellationToken);
        }

        public async Task<LocationDTO> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await UoW.Repository<IApplicationDbContext, Location>().GetFirstAsync(t => t.UrlSlug.Equals(urlSlug));
                return Mapper.Map<LocationDTO>(result);
            }
        }

        public override Task UpdateAsync(LocationDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.UpdateAsync(dto, cancellationToken);
        }


    }
}