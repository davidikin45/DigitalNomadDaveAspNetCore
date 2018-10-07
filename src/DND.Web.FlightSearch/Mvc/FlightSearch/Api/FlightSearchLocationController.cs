using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Helpers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Domain.ViewModels;
using DND.Interfaces.FlightSearch.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.FlightSearch.Mvc.Api
{
    [ApiVersion("1.0")]
    [Route("api/flight-search/location")]
    public class FlightSearchLocationController : ApiControllerBase
    {
        private readonly IFlightSearchApplicationService _flightSearchService;

        public FlightSearchLocationController(IFlightSearchApplicationService flightSearchService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, AppSettings appSettings, IAuthorizationService authorizationService)
             : base("flight.search.location.", mapper, emailService, urlHelper, appSettings, authorizationService)
        {
            _flightSearchService = flightSearchService;
        }


        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetByID([FromBody]LocationForm requestForm, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid) //Because form object implements IValidatableObject the validation has already occured!
                return ValidationErrors(ModelState);

            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var request = Mapper.Map<LocationForm, LocationRequestDto>(requestForm);
            var response = await _flightSearchService.GetLocationAsync(request, cts.Token);

            return Success(response);
        }

        [Route("auto-suggest")]
        [HttpGet]
        public async Task<IActionResult> LocationAutoSuggest([FromBody]LocationAutoSuggestForm requestForm, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid) //Because form object implements IValidatableObject the validation has already occured!
                return ValidationErrors(ModelState);

            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());
            var request = Mapper.Map<LocationAutoSuggestForm, LocationAutoSuggestRequestDto>(requestForm);
            var response = await _flightSearchService.LocationAutoSuggestAsync(request, cts.Token);

            return Success(response);
        }
    }
}