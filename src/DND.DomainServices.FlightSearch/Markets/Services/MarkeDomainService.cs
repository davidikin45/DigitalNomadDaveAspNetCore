using DND.Common.DomainServices;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Common.Infrastructure.Validation.Errors;
using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.FlightSearch.Markets.Services
{
    public class MarketDomainService : DomainServiceBase, IMarketDomainService
    {
        public MarketDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Country> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetMarketByIDAsync(id, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Result<List<Country>>> GetByLocale(string locale, CancellationToken cancellationToken)
        {
            if (locale == null)
                return Result.ObjectValidationFail<List<Country>>("Invaid Request");

            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return Result.Ok((await marketSearchEngine.GetMarketsByLocaleAsync(locale, cancellationToken).ConfigureAwait(false)).Countries);
        }
   
    }
}
