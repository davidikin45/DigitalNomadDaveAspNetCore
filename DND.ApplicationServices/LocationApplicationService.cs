using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;
using Solution.Base.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class LocationApplicationService : BaseEntityApplicationService<IApplicationDbContext, Location, LocationDTO>, ILocationApplicationService
    {
        public LocationApplicationService(ILocationDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
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
            var bo = await DomainService.GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(urlSlug));
            return Mapper.Map<LocationDTO>(bo);
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