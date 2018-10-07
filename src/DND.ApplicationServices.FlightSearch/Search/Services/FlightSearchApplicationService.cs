using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Common.Infrastructure.Validation;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Interfaces.FlightSearch.DomainServices;
using Microsoft.AspNetCore.Authorization;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.FlightSearch.Search.Services
{
    public class FlightSearchApplicationService : ApplicationServiceBase, IFlightSearchApplicationService
    {
        protected IFlightSearchDomainService DomainService { get; }
        public FlightSearchApplicationService(IFlightSearchDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService)
            : base("flight-search.flight-search.", mapper, authorizationService, userService)
        {
            DomainService = domainService;
        }

        public async Task<Result<FlightSearchResponseDto>> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken)
        {
            return await DomainService.SearchAsync(request, cancellationToken); 
        }

        public async Task<Result<LocationAutoSuggestResponseDto>> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken)
        {
            return await DomainService.LocationAutoSuggestAsync(request, cancellationToken);
        }

        public async Task<Result<LocationResponseDto>> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken)
        {
            return await DomainService.GetLocationAsync(request, cancellationToken);
        }
    }
}
