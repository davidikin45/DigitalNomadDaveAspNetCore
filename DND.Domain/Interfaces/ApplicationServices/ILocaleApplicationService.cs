using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.FlightSearch.Currencies.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ICurrencyApplicationService : IBaseApplicationService
    {
        Task<IEnumerable<CurrencyDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<CurrencyDto> GetAsync(string id, CancellationToken cancellationToken);
    }
}
