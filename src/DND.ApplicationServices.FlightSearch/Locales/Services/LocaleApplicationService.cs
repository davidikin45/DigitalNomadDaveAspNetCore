using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.FlightSearch.Locales.Dtos;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Interfaces.FlightSearch.DomainServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.FlightSearch.Locales.Services
{
    public class LocaleApplicationService : BaseApplicationService, ILocaleApplicationService
    {
        protected ILocaleDomainService DomainService { get; }

        public LocaleApplicationService(ILocaleDomainService domainService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
        }

        public async Task<LocaleDto> GetAsync(string id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAsync(id, cancellationToken);

            return Mapper.Map<Domain.Skyscanner.Model.Locale, LocaleDto>(bo);
        }

        public async Task<IEnumerable<LocaleDto>> GetAllAsync(CancellationToken cancellationToken)
        {

            var list = await DomainService.GetAllAsync(cancellationToken);

            var dtos = list.ToList().Select(Mapper.Map<DND.Domain.Skyscanner.Model.Locale, LocaleDto>);

            return dtos;
        }
    }
}
