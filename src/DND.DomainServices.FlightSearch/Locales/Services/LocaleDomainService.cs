using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.DomainServices.SearchEngines;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.FlightSearch.Locales.Services
{
    public class LocaleDomainService : DomainServiceBase, ILocaleDomainService
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
