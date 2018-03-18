using DND.Domain.DTOs;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface IFlightSearchDomainService : IBaseDomainService
    {
        Task<FlightSearchResponseDTO> SearchAsync(FlightSearchRequestDTO request, CancellationToken cancellationToken);

        Task<LocationAutoSuggestResponseDTO> LocationAutoSuggestAsync(LocationAutoSuggestRequestDTO request, CancellationToken cancellationToken);
        Task<LocationResponseDTO> GetLocationAsync(LocationRequestDTO request, CancellationToken cancellationToken);
    }
}
