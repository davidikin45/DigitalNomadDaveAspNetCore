using AutoMapper;
using HtmlTags;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using DND.Common.Alerts;
using DND.Common.Email;
using DND.Common.Enums;
using DND.Common.Extensions;
using DND.Common.Helpers;
using DND.Common.Implementation.Dtos;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DND.Common.Interfaces.Dtos;

namespace DND.Common.Controllers.Api
{
    //Resource naming - Should be noun = thing, not an action. Example. api/getauthors is bad. api/authors is good.
    //Folow this principle for predictability. Represent hierachy. api/authors/{author}/books
    //Pluralize vs not pluralize is OK. Stay consistent
    //Filters,Sorting,Ordering are not resources. Should not be in URL, use query string instead.


    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    public abstract class BaseEntityReadOnlyWebApiController<TDto, IEntityService> : BaseWebApiController
        where TDto : class, IBaseDtoWithId
        where IEntityService : IBaseEntityReadOnlyApplicationService<TDto>
    {
        public IEntityService Service { get; private set; }
        public ITypeHelperService TypeHelperService { get; private set; }

        public BaseEntityReadOnlyWebApiController(IEntityService service, IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, ITypeHelperService typeHelperService = null)
        : base(mapper, emailService, urlHelper)
        {
            Service = service;
            TypeHelperService = typeHelperService;
        }

        //http://jakeydocs.readthedocs.io/en/latest/mvc/models/formatting.html
        [FormatFilter]
        [Route("{id}"), Route("{id}.{format}")]
        //[Route("get/{id}"), Route("get/{id}.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public virtual async Task<IActionResult> Get(string id, [FromQuery] string fields)
        {
            if (!TypeHelperService.TypeHasProperties<TDto>(fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetByIdAsync(id, cts.Token);

            if (response == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var links = CreateLinks(id, fields);

            var linkedResourceToReturn = response.ShapeData(fields)
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);

            // Success(shapedData);
        }

        [FormatFilter]
        [Route("({ids})"), Route("({ids}).{format}")]
        //[Route("get/({ids})"), Route("get/({ids}).{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<object>), 200)]
        public virtual async Task<IActionResult> GetCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetByIdAsync(ids, cts.Token);

            var list = response.ToList();

            if (ids.Count() != list.Count())
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            return Success(list);
        }

        [FormatFilter]
        [Route("get-all")]
        [Route("get-all.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<object>), 200)]
        public virtual async Task<IActionResult> GetAll()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetAllAsync(cts.Token);

            var list = response.ToList();

            return Success(list);
        }

        [FormatFilter]
        [Route("")]
        [Route(".{format}")]
        //[Route("get-paged")]
        //[Route("get-paged.{format}")]
        [HttpGet]
        //[HttpPost]
        [HttpHead]
        [ProducesResponseType(typeof(WebApiPagedResponsedto<object>), 200)]
        public virtual async Task<IActionResult> GetPaged(WebApiPagedRequestDto resourceParameters)
        {
            if (string.IsNullOrEmpty(resourceParameters.OrderBy))
                resourceParameters.OrderBy = "id";

            if (!TypeHelperService.TypeHasProperties<TDto>(resourceParameters.Fields))
            {
                return ApiErrorMessage(Messages.FieldsInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetAllAsync(cts.Token, LamdaHelper.GetOrderBy<TDto>(resourceParameters.OrderBy, resourceParameters.OrderType), resourceParameters.Page - 1, resourceParameters.PageSize);

            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            //var response = new WebApiPagedResponsedto<TDto>
            //{
            //    Page = jqParams.Page,
            //    PageSize = jqParams.PageSize,
            //    Records = total,
            //    Rows = data.ToList()
            //};

            var paginationMetadata = new WebApiPagedResponsedto<TDto>
            {
                Page = resourceParameters.Page,
                PageSize = resourceParameters.PageSize,
                Records = total,
                PreviousPageLink = null,
                NextPageLink = null
            };

            if (paginationMetadata.HasPrevious)
            {
                paginationMetadata.PreviousPageLink = CreateResourceUri(resourceParameters, ResourceUriType.PreviousPage);
            }

            if (paginationMetadata.HasNext)
            {
                paginationMetadata.NextPageLink = CreateResourceUri(resourceParameters, ResourceUriType.NextPage);
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            var links = CreateLinksForCollections(resourceParameters,
              paginationMetadata.HasNext, paginationMetadata.HasPrevious);

            var shapedData = IEnumerableExtensions.ShapeData(data.ToList(), resourceParameters.Fields);

            var shapedDataWithLinks = shapedData.Select(author =>
            {
                var authorAsDictionary = author as IDictionary<string, object>;
                var authorLinks = CreateLinks(
                    authorAsDictionary["Id"].ToString(), resourceParameters.Fields);

                authorAsDictionary.Add("links", authorLinks);

                return authorAsDictionary;
            });

            var linkedCollectionResource = new
            {
                value = shapedDataWithLinks,
                links = links
            };

            return Ok(linkedCollectionResource);
        }

        [FormatFilter]
        [Route("get-all-paged")]
        [Route("get-all-paged.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(WebApiPagedResponsedto<object>), 200)]
        public virtual async Task<IActionResult> GetAllPaged()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dataTask = Service.GetAllAsync(cts.Token);

            var totalTask = Service.GetCountAsync(cts.Token);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            //var response = new WebApiPagedResponsedto<TDto>
            //{
            //    CurrentPage = 1,
            //    PageSize = total,
            //    Records = total,
            //    Rows = data.ToList()
            //};

            var paginationMetadata = new WebApiPagedResponsedto<TDto>
            {
                Page = 1,
                PageSize = total,
                Records = total,
                PreviousPageLink = "",
                NextPageLink = ""
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));

            return Success(data.ToList());
        }

        /// <summary>
        /// Tagses the HTML.
        /// </summary>
        /// <returns></returns>
        [FormatFilter]
        [Route("get-all-html")]
        [Route("get-all-html.{format}")]
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

        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, PATCH");
            return Ok();
        }

        #region HATEOAS
        private string CreateResourceUri(
WebApiPagedRequestDto resourceParameters,
ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return UrlHelper.Action(nameof(GetPaged),
                          UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          fields = resourceParameters.Fields,
                          orderBy = resourceParameters.OrderBy,
                          search = resourceParameters.Search,
                          page = resourceParameters.Page - 1,
                          pageSize = resourceParameters.PageSize
                      },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);
                case ResourceUriType.NextPage:
                    return UrlHelper.Action(nameof(GetPaged),
                          UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                      new
                      {
                          fields = resourceParameters.Fields,
                          orderBy = resourceParameters.OrderBy,
                          search = resourceParameters.Search,
                          page = resourceParameters.Page + 1,
                          pageSize = resourceParameters.PageSize
                      },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);

                default:
                    return UrlHelper.Action(nameof(GetPaged),
                    UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                    new
                    {
                        fields = resourceParameters.Fields,
                        orderBy = resourceParameters.OrderBy,
                        search = resourceParameters.Search,
                        page = resourceParameters.Page,
                        pageSize = resourceParameters.PageSize
                    },
                      UrlHelper.ActionContext.HttpContext.Request.Scheme);
            }
        }

        private IEnumerable<LinkDto> CreateLinks(string id, string fields)
        {
            var links = new List<LinkDto>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                  new LinkDto(UrlHelper.Action(nameof(Get), UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  "GET"));
            }
            else
            {
                links.Add(
                  new LinkDto(UrlHelper.Action(nameof(Get), UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id, fields = fields }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                  "self",
                  "GET"));
            }

            links.Add(
              new LinkDto(UrlHelper.Action("Delete", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(), new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
              "delete",
              "DELETE"));

            links.Add(
                new LinkDto(UrlHelper.Action("Update", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                "update",
                "PUT"));

            links.Add(
                new LinkDto(UrlHelper.Action("UpdatePartial", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
                new { id = id }, UrlHelper.ActionContext.HttpContext.Request.Scheme),
                "partially_update",
                "PATCH"));

            return links;
        }

        private  IEnumerable<LinkDto> CreateLinksForCollections(WebApiPagedRequestDto resourceParameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDto>();

            // self 
            links.Add(
               new LinkDto(CreateResourceUri(resourceParameters,
               ResourceUriType.Current)
               , "self", "GET"));

            links.Add(
           new LinkDto(UrlHelper.Action("Create", UrlHelper.ActionContext.RouteData.Values["controller"].ToString(),
          null, UrlHelper.ActionContext.HttpContext.Request.Scheme),
           "add",
           "POST"));

            if (hasNext)
            {
                links.Add(
                  new LinkDto(CreateResourceUri(resourceParameters,
                  ResourceUriType.NextPage),
                  "nextPage", "GET"));
            }

            if (hasPrevious)
            {
                links.Add(
                    new LinkDto(CreateResourceUri(resourceParameters,
                    ResourceUriType.PreviousPage),
                    "previousPage", "GET"));
            }

            return links;
        }
        #endregion

    }
}

