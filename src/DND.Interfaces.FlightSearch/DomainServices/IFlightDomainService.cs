using DND.Common.Interfaces.DomainServices;
using DND.Domain.FlightSearch.Search.Dtos;
using System.Threading;
using System.Threading.Tasks;


namespace DND.Interfaces.FlightSearch.DomainServices
{
    public interface IFlightSearchDomainService : IBaseDomainService
    {
        Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken);

        Task<LocationAutoSuggestResponseDto> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken);
        Task<LocationResponsedto> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken);
    }
}
