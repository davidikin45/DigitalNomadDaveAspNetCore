using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class CurrencyApplicationService : BaseDomainService, ICurrencyDomainService
    {
        public CurrencyApplicationService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Currency> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetCurrencyByIDAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Currency>> GetAllAsync(CancellationToken cancellationToken)
        {

            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return (await marketSearchEngine.GetCurrenciesAsync(cancellationToken)).Currencies;
        }
    }
}
