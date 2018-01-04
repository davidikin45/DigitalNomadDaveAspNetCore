using DND.Domain.DTOs;
using Solution.Base.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.Services
{
    public interface IFlightSearchService : IBaseBusinessService
    {
        Task<FlightSearchResponseDTO> SearchAsync(FlightSearchRequestDTO request, CancellationToken cancellationToken);

        Task<LocationAutoSuggestResponseDTO> LocationAutoSuggestAsync(LocationAutoSuggestRequestDTO request, CancellationToken cancellationToken);
        Task<LocationResponseDTO> GetLocationAsync(LocationRequestDTO request, CancellationToken cancellationToken);
    }
}
