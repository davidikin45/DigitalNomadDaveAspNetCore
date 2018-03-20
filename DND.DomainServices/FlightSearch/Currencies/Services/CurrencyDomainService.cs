using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines;
using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.FlightSearch.Currencies.Services
{
    public class CurrencyApplicationService : BaseDomainService, ICurrencyDomainService
    {
        public CurrencyApplicationService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<DND.Domain.Skyscanner.Model.Currency> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetCurrencyByIDAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<DND.Domain.Skyscanner.Model.Currency>> GetAllAsync(CancellationToken cancellationToken)
        {

            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return (await marketSearchEngine.GetCurrenciesAsync(cancellationToken)).Currencies;
        }
    }
}
