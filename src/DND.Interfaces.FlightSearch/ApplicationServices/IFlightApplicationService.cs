using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.FlightSearch.Search.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.ApplicationServices
{
    public interface IFlightSearchApplicationService : IApplicationService
    {
        Task<FlightSearchResponseDto> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken);

        Task<LocationAutoSuggestResponseDto> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken);
        Task<LocationResponseDto> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken);
    }
}
