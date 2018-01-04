using AutoMapper;
using HtmlTags;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Email;
using Solution.Base.Extensions;
using Solution.Base.Helpers;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solution.Base.Controllers.Api
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    public abstract class BaseEntityReadOnlyWebApiController<TDto, IEntityService> : BaseWebApiController
        where TDto : class, IBaseEntity
        where IEntityService : IBaseEntityService<TDto>
    {   
        public IEntityService Service { get; private set; }

        public BaseEntityReadOnlyWebApiController(IEntityService service, IMapper mapper = null,  IEmailService emailService = null)
        : base(mapper, emailService)
        {
            Service = service;
        }

        [Route("{id}"), Route("get/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(IBaseEntity),200)]
        public virtual async Task<IActionResult> Get(int id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

           var response =  await Service.GetByIdAsync(id, cts.Token);
           return Success(response);
        }

        [Route("get-all")]
        [HttpGet]
        [ProducesResponseType(typeof(List<IBaseEntity>), 200)]
        public virtual async Task<IActionResult> GetAll()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetAllAsync(cts.Token);

            var list = response.ToList();

            return Success(list);
        }

        [Route("get-paged")]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiPagedResponseDTO<IBaseEntity>), 200)]
        public virtual async Task<IActionResult> GetPaged(WebApiPagedRequestDTO jqParams)
        {
            if (string.IsNullOrEmpty(jqParams.OrderBy))
                jqParams.OrderBy = "id";

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<TDto>(jqParams.OrderBy, jqParams.OrderType),jqParams.Page - 1, jqParams.PageSize);

            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var response = new WebApiPagedResponseDTO<TDto>
            {
                Page = jqParams.Page,
                PageSize = jqParams.PageSize,
                Records = total,
                Rows = data.ToList()
            };

            return Success(response);
        }


        [Route("get-all-paged")]
        [HttpGet]
        [ProducesResponseType(typeof(WebApiPagedResponseDTO<IBaseEntity>), 200)]
        public virtual async Task<IActionResult> GetAllPaged()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetAllAsync(cts.Token);

            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var response = new WebApiPagedResponseDTO<TDto>
            {
                Page = 1,
                PageSize = total,
                Records = total,
                Rows = data.ToList()
            };

            return Success(response);
        }

        /// <summary>
        /// Tagses the HTML.
        /// </summary>
        /// <returns></returns>
        [Route("get-all-html")]
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<IActionResult> GetAllHtml()
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var data = await Service.GetAllAsync(cts.Token);

            var select = new HtmlTag("select");

            foreach (var item in data)
            {
                var description = item.Id.ToString();
                
                if (item.HasProperty("Name") && !string.IsNullOrEmpty((string)item.GetPropValue("Name")))
                {
                    description = (string)item.GetPropValue("Name");
                }

                select.Append(new HtmlTag("option").Text(description).Value(item.Id.ToString()));
            }

            return Html(select.ToString());
        }

    }
}

