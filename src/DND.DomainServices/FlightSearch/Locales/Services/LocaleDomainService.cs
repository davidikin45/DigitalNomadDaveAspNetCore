using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Domain.Interfaces.DomainServices;
using DND.DomainServices.SearchEngines;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.FlightSearch.Locales.Services
{
    public class LocaleDomainService : BaseDomainService, ILocaleDomainService
    {

        public LocaleDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {
          
        }

        public async Task<DND.Domain.Skyscanner.Model.Locale> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetLocaleByIDAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<DND.Domain.Skyscanner.Model.Locale>> GetAllAsync(CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return (await marketSearchEngine.GetLocalesAsync(cancellationToken).ConfigureAwait(false)).Locales;
        }
    }
}
