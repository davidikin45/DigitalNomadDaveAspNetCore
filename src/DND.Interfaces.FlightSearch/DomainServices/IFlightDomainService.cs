using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Common.Infrastructure.Validation;
using DND.Domain.FlightSearch.Search.Dtos;
using System.Threading;
using System.Threading.Tasks;


namespace DND.Interfaces.FlightSearch.DomainServices
{
    public interface IFlightSearchDomainService : IDomainService
    {
        Task<Result<FlightSearchResponseDto>> SearchAsync(FlightSearchRequestDto request, CancellationToken cancellationToken);

        Task<Result<LocationAutoSuggestResponseDto>> LocationAutoSuggestAsync(LocationAutoSuggestRequestDto request, CancellationToken cancellationToken);
        Task<Result<LocationResponseDto>> GetLocationAsync(LocationRequestDto request, CancellationToken cancellationToken);
    }
}
