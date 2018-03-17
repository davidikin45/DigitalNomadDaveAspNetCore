using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using Solution.Base.Extensions;
using Solution.Base.Implementation.ApplicationServices;
using Solution.Base.Implementation.Validation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class FlightSearchApplicationService : BaseApplicationService, IFlightSearchApplicationService
    {
        protected IFlightSearchDomainService DomainService { get; }
        public FlightSearchApplicationService(IFlightSearchDomainService domainService, IMapper mapper)
            : base(mapper)
        {
            DomainService = domainService;
        }

        public async Task<FlightSearchResponseDTO> SearchAsync(FlightSearchRequestDTO request, CancellationToken cancellationToken)
        {
            return await DomainService.SearchAsync(request, cancellationToken); 
        }

        public async Task<LocationAutoSuggestResponseDTO> LocationAutoSuggestAsync(LocationAutoSuggestRequestDTO request, CancellationToken cancellationToken)
        {
            return await DomainService.LocationAutoSuggestAsync(request, cancellationToken);
        }

        public async Task<LocationResponseDTO> GetLocationAsync(LocationRequestDTO request, CancellationToken cancellationToken)
        {
            return await DomainService.GetLocationAsync(request, cancellationToken);
        }
    }
}
