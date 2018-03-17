using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Email;
using Solution.Base.Helpers;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Controllers
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
    [Route("api/flight-search")]
    public class FlightSearchController : BaseWebApiController
    {
        private readonly IFlightSearchApplicationService _flightSearchService;

        public FlightSearchController(IFlightSearchApplicationService flightSearchService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper)
            : base(mapper, emailService, urlHelper)
        {

            _flightSearchService = flightSearchService;
        }

        //[Route("")] - Only required when method needs to be accesed by route prefix. can still access via default route.
        //[Route("~/flight-search")]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //[NoAsyncTimeout]
        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(FlightSearchResponseDTO), 200)]
        public virtual async Task<IActionResult> Search(FlightSearchClientRequestForm requestForm, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var request = Mapper.Map<FlightSearchClientRequestForm, FlightSearchRequestDTO>(requestForm);
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