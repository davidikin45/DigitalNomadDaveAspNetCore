using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.FlightSearch.Currencies.Dtos;
using DND.Domain.FlightSearch.Markets.Dtos;
using DND.Domain.Skyscanner.Model;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.FlightSearch.Markets.Services
{
    public class MarketApplicationService : BaseApplicationService, IMarketApplicationService
    {
        protected IMarketDomainService DomainService { get; }
        protected ICurrencyApplicationService CurrencyApplicationService { get; }

        public MarketApplicationService(IMarketDomainService domainService, ICurrencyApplicationService currencyApplicationService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
            CurrencyApplicationService = currencyApplicationService;
        }

        public async Task<MarketDto> GetAsync(string id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAsync(id, cancellationToken);
            var dto = Mapper.Map<Domain.Skyscanner.Model.Country, MarketDto>(bo);
            await Formatdto(dto, cancellationToken);
            return dto;
        }

        public async Task<IEnumerable<MarketDto>> GetByLocale(string locale, CancellationToken cancellationToken)
        {

            var response = await DomainService.GetByLocale(locale, cancellationToken);

            var list = new List<MarketDto>();

            foreach (Domain.Skyscanner.Model.Country c in response)
            {
                var dto = Mapper.Map<Country, MarketDto>(c);
                await Formatdto(dto, cancellationToken);
                list.Add(dto);
            }

            return list;
        }

        private async Task Formatdto(MarketDto dto, CancellationToken cancellationToken)
        {
            Task<CurrencyDto> currency = null;

            if (!string.IsNullOrEmpty(dto.CurrencyId))
            {
                currency = CurrencyApplicationService.GetAsync(dto.CurrencyId, cancellationToken);
            }

            if(currency != null)
            {
                dto.Currency = await currency;
            }

        }
   
    }
}
