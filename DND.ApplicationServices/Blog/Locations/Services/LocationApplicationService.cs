using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure;
using DND.Domain.Blog.Locations;
using DND.Domain.Blog.Locations.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.Locations.Services
{
    public class LocationApplicationService : BaseEntityApplicationService<IApplicationDbContext, Location, LocationDto, LocationDto, LocationDto, LocationDto, ILocationDomainService>, ILocationApplicationService
    {
        public LocationApplicationService(ILocationDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }

        public override Task<LocationDto> CreateAsync(LocationDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.CreateAsync(dto, cancellationToken);
        }

        public async Task<LocationDto> GetLocationAsync(string urlSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetFirstAsync(cancellationToken, t => t.UrlSlug.Equals(urlSlug));
            return Mapper.Map<LocationDto>(bo);
        }

        public override Task UpdateAsync(object id, LocationDto dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return base.UpdateAsync(id, dto, cancellationToken);
        }


    }
}