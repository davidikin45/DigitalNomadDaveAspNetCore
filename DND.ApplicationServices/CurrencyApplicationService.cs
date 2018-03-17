using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using Solution.Base.Implementation.ApplicationServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class CurrencyApplicationService : BaseApplicationService, ICurrencyApplicationService
    {
        protected ICurrencyDomainService DomainService { get; }

        public CurrencyApplicationService(ICurrencyDomainService domainService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
        }

        public async Task<CurrencyDTO> GetAsync(string id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetAsync(id, cancellationToken);

            return Mapper.Map<Domain.Skyscanner.Model.Currency, CurrencyDTO>(bo);
        }

        public async Task<IEnumerable<CurrencyDTO>> GetAllAsync(CancellationToken cancellationToken)
        {
            var list = await DomainService.GetAllAsync(cancellationToken);

            var dtos = list.ToList().Select(Mapper.Map<Currency, CurrencyDTO>);

            return dtos;
        }
    }
}
