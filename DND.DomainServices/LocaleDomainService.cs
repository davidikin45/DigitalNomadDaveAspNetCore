using DND.Domain.DTOs;
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
    public class LocaleDomainService : BaseDomainService, ILocaleDomainService
    {

        public LocaleDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {
          
        }

        public async Task<Locale> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetLocaleByIDAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Locale>> GetAllAsync(CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return (await marketSearchEngine.GetLocalesAsync(cancellationToken)).Locales;
        }
    }
}
