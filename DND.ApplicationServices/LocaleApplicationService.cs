using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using DND.Common.Implementation.ApplicationServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class LocaleApplicationService : BaseApplicationService, ILocaleApplicationService
    {
        protected ILocaleDomainService DomainService { get; }

        public LocaleApplicationService(ILocaleDomainService domainService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
        }

        public async Task<LocaleDTO> GetAsync(string id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAsync(id, cancellationToken);

            return Mapper.Map<Domain.Skyscanner.Model.Locale, LocaleDTO>(bo);
        }

        public async Task<IEnumerable<LocaleDTO>> GetAllAsync(CancellationToken cancellationToken)
        {

            var list = await DomainService.GetAllAsync(cancellationToken);

            var dtos = list.ToList().Select(Mapper.Map<Locale, LocaleDTO>);

            return dtos;
        }
    }
}
