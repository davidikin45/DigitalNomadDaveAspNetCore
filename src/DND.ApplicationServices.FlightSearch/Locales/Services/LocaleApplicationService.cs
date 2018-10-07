using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Infrastructure.Users;
using DND.Domain.FlightSearch.Locales.Dtos;
using DND.Interfaces.FlightSearch.ApplicationServices;
using DND.Interfaces.FlightSearch.DomainServices;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.FlightSearch.Locales.Services
{
    public class LocaleApplicationService : ApplicationServiceBase, ILocaleApplicationService
    {
        protected ILocaleDomainService DomainService { get; }

        public LocaleApplicationService(ILocaleDomainService domainService, IMapper mapper, IAuthorizationService authorizationService, IUserService userService)
            : base("flight-serch.locales.", mapper, authorizationService, userService)
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
