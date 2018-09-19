using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.FlightSearch.Search.Services
{
    public class FlightSearchApplicationService : ApplicationServiceBase, IFlightSearchApplicationService
    {
        protected IFlightSearchDomainService DomainService { get; }
        public FlightSearchApplicationService(IFlightSearchDomainService domainService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
        }

        public async Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken)
        {
            return await DomainService.SearchAsync(request, cancellationToken); 
        }

        public async Task<LocationAutoSuggestResponseDto> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken)
        {
            return await DomainService.LocationAutoSuggestAsync(request, cancellationToken);
        }

        public async Task<LocationResponseDto> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken)
        {
            return await DomainService.GetLocationAsync(request, cancellationToken);
        }
    }
}
