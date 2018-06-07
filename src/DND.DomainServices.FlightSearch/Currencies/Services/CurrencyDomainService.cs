using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.FlightSearch.Currencies.Services
{
    public class CurrencyDomainService : BaseDomainService, ICurrencyDomainService
    {
        public CurrencyDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Currency> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetCurrencyByIDAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken)
        {

            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return (await marketSearchEngine.GetCurrenciesAsync(cancellationToken).ConfigureAwait(false)).Currencies;
        }
    }
}
