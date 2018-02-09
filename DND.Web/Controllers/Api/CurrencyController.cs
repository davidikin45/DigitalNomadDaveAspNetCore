﻿using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers.Api;
using Solution.Base.Helpers;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/currency")]
    public class CurrencyController : BaseWebApiController
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService, IMapper mapper)
             : base(mapper)
        {

            _currencyService = currencyService;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(List<CurrencyDTO>), 200)]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken = default(CancellationToken))
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var response = await _currencyService.GetAllAsync(cts.Token);

            var list = response.ToList();

            return Success(list);
        }

        [Route("{id:alpha}")]
        [HttpGet]
        [ProducesResponseType(typeof(CurrencyDTO), 200)]
        public virtual async Task<IActionResult> Get(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(cancellationToken, ClientDisconnectedToken());

            var response = await _currencyService.GetAsync(id, cts.Token);

            return Success(response);
        }
    }
}