using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.Validation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
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

        public async Task<MarketDTO> GetAsync(string id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAsync(id, cancellationToken);
            var DTO = Mapper.Map<Domain.Skyscanner.Model.Country, MarketDTO>(bo);
            await FormatDTO(DTO, cancellationToken);
            return DTO;
        }

        public async Task<IEnumerable<MarketDTO>> GetByLocale(string locale, CancellationToken cancellationToken)
        {

            var response = await DomainService.GetByLocale(locale, cancellationToken);

            var list = new List<MarketDTO>();

            foreach (Domain.Skyscanner.Model.Country c in response)
            {
                var DTO = Mapper.Map<Country, MarketDTO>(c);
                await FormatDTO(DTO, cancellationToken);
                list.Add(DTO);
            }

            return list;
        }

        private async Task FormatDTO(MarketDTO DTO, CancellationToken cancellationToken)
        {
            Task<CurrencyDTO> currency = null;

            if (!string.IsNullOrEmpty(DTO.CurrencyId))
            {
                currency = CurrencyApplicationService.GetAsync(DTO.CurrencyId, cancellationToken);
            }

            if(currency != null)
            {
                DTO.Currency = await currency;
            }

        }
   
    }
}
