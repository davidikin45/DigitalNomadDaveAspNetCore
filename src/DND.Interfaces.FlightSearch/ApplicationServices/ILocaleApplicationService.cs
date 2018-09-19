using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.FlightSearch.Currencies.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.ApplicationServices
{
    public interface ICurrencyApplicationService : IApplicationService
    {
        Task<IEnumerable<CurrencyDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<CurrencyDto> GetAsync(string id, CancellationToken cancellationToken);
    }
}
