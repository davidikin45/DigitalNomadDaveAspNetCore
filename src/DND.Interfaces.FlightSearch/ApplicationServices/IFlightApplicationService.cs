using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Common.Infrastructure.Validation;
using DND.Domain.FlightSearch.Search.Dtos;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.ApplicationServices
{
    public interface IFlightSearchApplicationService : IApplicationService
    {
        Task<Result<FlightSearchResponseDto>> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken);

        Task<Result<LocationAutoSuggestResponseDto>> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken);
        Task<Result<LocationResponseDto>> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken);
    }
}
