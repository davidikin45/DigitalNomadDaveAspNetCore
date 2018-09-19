using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.FlightSearch.Currencies.Dtos;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.FlightSearch.Currencies.Services
{
    public class CurrencyApplicationService : ApplicationServiceBase, ICurrencyApplicationService
    {
        protected ICurrencyDomainService DomainService { get; }

        public CurrencyApplicationService(ICurrencyDomainService domainService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
        }

        public async Task<CurrencyDto> GetAsync(string id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAsync(id, cancellationToken);

            return Mapper.Map<Domain.Skyscanner.Model.Currency, CurrencyDto>(bo);
        }

        public async Task<IEnumerable<CurrencyDto>> GetAllAsync(CancellationToken cancellationToken)
        {
            var list = await DomainService.GetAllAsync(cancellationToken);

            var dtos = list.ToList().Select(Mapper.Map<DND.Domain.Skyscanner.Model.Currency, CurrencyDto>);

            return dtos;
        }
    }
}
