﻿using AutoMapper;
using DND.Common.Controllers.Api;
using DND.Common.Helpers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Domain.FlightSearch.Locales.Dtos;
using DND.Interfaces.FlightSearch.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/flight-search/locales")]
    public class LocalesController : ApiControllerBase
    {
        private readonly ILocaleApplicationService _localeService;

        public LocalesController(ILocaleApplicationService localeService, IMapper mapper, IEmailService emailService, IUrlHelper urlHelper, AppSettings appSettings, IAuthorizationService authorizationService)
             : base("flight-search.locales.", mapper, emailService, urlHelper, appSettings, authorizationService)
        {
            _localeService = localeService;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(IList<LocaleDto>), 200)]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken = default(CancellationToken))
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var response = await _localeService.GetAllAsync(cts.Token);

            var list = response.ToList();

            return Success(list);
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(LocaleDto), 200)]
        public virtual async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var response = await _localeService.GetAsync(id, cts.Token);

            return Success(response);
        }
    }
}