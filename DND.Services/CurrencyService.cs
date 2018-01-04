using DND.Core.Interfaces.Services;
using DND.Core.Models;
using Solution.Base.Implementation.Services;
using Solution.Base.Implementation.Validation;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

using DND.Services.Skyscanner.Model;
using DND.Core.DTOs;
using DND.Services.SearchEngines;
using AutoMapper;
using System.Threading;

namespace DND.Services
{
    public class CurrencyService : BaseBusinessService, ICurrencyService
    {
        public CurrencyService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
            : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public async Task<CurrencyDTO> GetAsync(string id, CancellationToken cancellationToken)
        {
            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            var response = await marketSearchEngine.GetCurrencyByIDAsync(id, cancellationToken);

            return Mapper.Map<Skyscanner.Model.Currency, CurrencyDTO>(response);
        }

        public async Task<IEnumerable<CurrencyDTO>> GetAllAsync(CancellationToken cancellationToken)
        {

            var marketSearchEngine = new LocaleMarketCurrencySearchEngine("skyscanner");

            var response = await marketSearchEngine.GetCurrenciesAsync(cancellationToken);

            var list = new List<CurrencyDTO>();

            foreach (Skyscanner.Model.Currency c in response.Currencies)
            {
                var DTO = Mapper.Map<Skyscanner.Model.Currency, CurrencyDTO>(c);
                list.Add(DTO);
            }

            return list;
        }
    }
}
