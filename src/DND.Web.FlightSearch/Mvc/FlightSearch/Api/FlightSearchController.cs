using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Helpers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.FlightSearch.Search.Dtos;
using DND.Domain.ViewModels;
using DND.Interfaces.FlightSearch.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.FlightSearch.Mvc.Api
{
    //webapi
    //    The algorithm ASP.NET uses to calculate the "default" method for a given action goes like this:
    //If there is an attribute applied (via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    //https://www.exceptionnotfound.net/using-http-methods-correctly-in-asp-net-web-api/
    //in webapi  HTTP methods GET, (POST, PUT), PATCH, DELETE help the routing CRUD

    //without web api json can't be returned with GET


    //cannot begin or end with /
    [ApiVersion("1.0")]
    [Route("api/flight-search/flight-search")]
    public class FlightSearchController : ApiControllerBase
    {
        private readonly IFlightSearchApplicationService _flightSearchService;

        public FlightSearchController(IFlightSearchApplicationService flightSearchService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, AppSettings appSettings)
            : base(mapper, emailService, urlHelper, appSettings)
        {

            _flightSearchService = flightSearchService;
        }

        //[Route("")] - Only required when method needs to be accesed by route prefix. can still access via default route.
        [HttpGet]
        [Route("~/flight-search")]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View());
        }

        [HttpGet]
        [Route("~/flight-search-results")]
        public async Task<IActionResult> Results()
        {
            return await Task.FromResult(View());
        }

        //[NoAsyncTimeout]
        [Route("")]
        [HttpPost]
        public virtual async Task<IActionResult> Search([FromBody]FlightSearchClientRequestForm requestForm, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var request = Mapper.Map<FlightSearchClientRequestForm, FlightSearchRequestDto>(requestForm);
            var response = await _flightSearchService.SearchAsync(request, cts.Token);

            return Success(response);
        }

        //[NonAction]  
        //int, alpha, datetime, boolean, decimal, double, range(10,50), min(10), max(10)
        //http://www.asp.net/web-api/overview/web-api-routing-and-actions/routing-in-aspnet-web-api
        //http://www.asp.net/web-api/overview/web-api-routing-and-actions/attribute-routing-in-web-api-2
        //https://www.exceptionnotfound.net/attribute-routing-vs-convention-routing/
        //[Route("search/{market:alpha}/{currency:alpha}/{locale:alpha}/{originPlace:alpha}/{destinationPlace:alpha}/{outboundPartialDate:datetime}/{inboundPartialDate:datetime?}/{adults:int}/{children:int}/{infants:int}/{maxStops:int:range(0,3)}/{priceFilter:double?}")]
        //http://partners.api.skyscanner.net/apiservices/browseroutes/v1.0/{market}/{currency}/{locale}/{originPlace}/{destinationPlace}/{outboundPartialDate}/{inboundPartialDate}
        //[NoAsyncTimeout]
    
    }
}