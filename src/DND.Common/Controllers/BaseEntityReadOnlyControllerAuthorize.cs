using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Email;
using DND.Common.Helpers;
using DND.Common.Implementation.Dtos;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Services;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DND.Common.Interfaces.Dtos;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using DND.Common.Extensions;
using DND.Common.Controllers.Api;

namespace DND.Common.Controllers
{
    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Policy = ApiScopes.Read)]
    public abstract class BaseEntityReadOnlyControllerAuthorize<TDto, IEntityService> : BaseController
        where TDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
        where IEntityService : IBaseEntityReadOnlyApplicationService<TDto>
    {   
        public IEntityService Service { get; private set; }
        public Boolean Admin { get; set; }

        public BaseEntityReadOnlyControllerAuthorize(Boolean admin, IEntityService service, IMapper mapper = null, IEmailService emailService = null, IConfiguration configuration = null)
        : base(mapper, emailService, configuration)
        {
            Admin = admin;
            Service = service;
        }

        // GET: Default
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(IBaseDtoWithId.Id), string orderType = OrderByType.Descending, string search = "")
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
                  
            try
            {
                var dataTask = Service.SearchAsync(cts.Token, search, null, LamdaHelper.GetOrderBy<TDto>(orderColumn, orderType), page - 1, pageSize, true, false, null);
                var totalTask = Service.GetSearchCountAsync(cts.Token,search);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDto<TDto>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = data.ToList(),
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search
                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("List", response);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }
 
        // GET: Default/Details/5
        [Route("details/{id}")]
        public virtual async Task<ActionResult> Details(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TDto data = null;
            try
            {
                data = await Service.GetByIdAsync(id, cts.Token, true);
                if (data == null)
                    return HandleReadException();
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Details", data);
        }

        [Route("details/{id}/{collection}")]
        public virtual async Task<ActionResult> Collection(string id, string collection, int page = 1, int pageSize = 10, string orderColumn = nameof(IBaseDtoWithId.Id), string orderType = OrderByType.Descending, string search = "")
        {
            if (!typeof(TDto).HasProperty(collection) || !typeof(TDto).IsCollectionProperty(collection))
            {
                return HandleReadException();
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var collectionItemType = typeof(TDto).GetGenericArguments(collection).Single();

                var dataTask = Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, page - 1, pageSize);

                var totalTask = Service.GetByIdWithPagedCollectionPropertyCountAsync(cts.Token, id, collection);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result.GetPropValue(collection);
                var total = totalTask.Result;

                var list = typeof(Enumerable).GetMethod(nameof(Enumerable.ToList)).MakeGenericMethod(collectionItemType).Invoke(null, new object[] { data });

                var webApiPagedResponseDtoType = typeof(WebApiPagedResponseDto<>).MakeGenericType(collectionItemType);
                var response = Activator.CreateInstance(webApiPagedResponseDtoType);

                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Page), page);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.PageSize), pageSize);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Records), total);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Rows), list);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.OrderColumn), orderColumn);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.OrderType), orderType);
                response.SetPropValue(nameof(WebApiPagedResponseDto<Object>.Search), search);

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                //For the time being collection properties are read only. DDD states that only the Aggregate Root should get updated.
                ViewBag.DisableEdit = true;
                ViewBag.DisableDelete = true;
                ViewBag.DisableSorting = true;
                ViewBag.DisableEntityEvents = true;

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("List", response);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        [Route("details/{id}/{collection}/{collectionItemId}")]
        public virtual async Task<ActionResult> CollectionItemDetails(string id, string collection, string collectionItemId)
        {
            if (!typeof(TDto).HasProperty(collection) || !typeof(TDto).IsCollectionProperty(collection))
            {
                return HandleReadException();
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            Object data = null;
            try
            {
                var collectionItemType = typeof(TDto).GetGenericArguments(collection).Single();

                var response = await Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, null, null, collectionItemId);

                if (response == null || response.GetPropValue(collection) == null)
                {
                    return HandleReadException();
                }

                var whereClause = LamdaHelper.SearchForEntityById(collectionItemType, collectionItemId);
                var collectionItem = typeof(LamdaHelper).GetMethod(nameof(LamdaHelper.FirstOrDefault)).MakeGenericMethod(collectionItemType).Invoke(null, new object[] { response.GetPropValue(collection), whereClause });

                if (collectionItem == null)
                {
                    return HandleReadException();
                }

                data = collectionItem;
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Details", data);
        }

        protected virtual TDto CreateNewDtoInstance()
        {
            return (TDto)Activator.CreateInstance(typeof(TDto));
        }

    }
}

