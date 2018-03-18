﻿using DND.Domain.DTOs;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using DND.DomainServices.SearchEngines;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Implementation.Validation;
using Solution.Base.Interfaces.UnitOfWork;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class MarketDomainService : BaseDomainService, IMarketDomainService
    {
        public MarketDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Country> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return await marketSearchEngine.GetMarketByIDAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<Country>> GetByLocale(string locale, CancellationToken cancellationToken)
        {
            if (locale == null)
                throw new ValidationErrors(new GeneralError("Invaid Request"));

            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            return (await marketSearchEngine.GetMarketsByLocaleAsync(locale, cancellationToken)).Countries;
        }
   
    }
}