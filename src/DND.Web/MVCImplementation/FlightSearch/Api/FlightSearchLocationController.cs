using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Email;
using DND.Common.Helpers;
using DND.Domain.FlightSearch.Markets.Dtos;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.FlightSearch.Api
{
    [ApiVersion("1.0")]
    [Route("api/flight-search-location")]
    public class FlightSearchLocationController : BaseWebApiController
    {
        private readonly IFlightSearchApplicationService _flightSearchService;

        public FlightSearchLocationController(IFlightSearchApplicationService flightSearchService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, IConfiguration configuration)
             : base(mapper, emailService, urlHelper, configuration)
        {
            _flightSearchService = flightSearchService;
        }


        [Route("")]
        public async Task<IActionResult> GetByID([FromBody]LocationForm requestForm, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (!ModelState.IsValid) //Because form object implements IValidatableObject the validation has already occured!
                    return ValidationErrors(ModelState);

                var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

                var request = Mapper.Map<LocationForm, LocationRequestDto>(requestForm);
                var response = await _flightSearchService.GetLocationAsync(request, cts.Token);

                return Success(response);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [Route("auto-suggest")]
        public async Task<IActionResult> LocationAutoSuggest([FromBody]LocationAutoSuggestForm requestForm, CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                if (!ModelState.IsValid) //Because form object implements IValidatableObject the validation has already occured!
                    return ValidationErrors(ModelState);

                var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());
                var request = Mapper.Map<LocationAutoSuggestForm, LocationAutoSuggestRequestDto>(requestForm);
                var response = await _flightSearchService.LocationAutoSuggestAsync(request, cts.Token);

                return Success(response);

            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}