using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Common.Infrastructure.Validation;
using DND.Domain.FlightSearch.Markets.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.ApplicationServices
{
    public interface IMarketApplicationService : IApplicationService
    {
        Task<Result<List<MarketDto>>> GetByLocale(string locale, CancellationToken cancellationToken);
        Task<MarketDto> GetAsync(string id, CancellationToken cancellationToken);
    }
}
