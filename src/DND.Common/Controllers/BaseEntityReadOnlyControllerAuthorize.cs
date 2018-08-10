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
using DND.Common.Implementation.Data;
using System.Dynamic;

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

        #region List
        // GET: Default
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(IBaseDtoWithId.Id), string orderType = OrderByType.Descending, string search = "")
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var dataTask = Service.SearchAsync(cts.Token, search, null, LamdaHelper.GetOrderBy<TDto>(orderColumn, orderType), page - 1, pageSize, true, false, null);
                var totalTask = Service.GetSearchCountAsync(cts.Token, search);

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
        #endregion

        #region Dynamic
        [Route("dynamic/{id}")]
        public virtual async Task<ActionResult> Dynamic(string id)
        {
            dynamic data = new ExpandoObject();
            data.Names = "Test";

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Details", data);
        }
        #endregion

        #region Collection List and Details

        [Route("details/{id}/{*collection}")]
        public virtual async Task<ActionResult> Collection(string id, string collection, int page = 1, int pageSize = 10, string orderColumn = nameof(IBaseDtoWithId.Id), string orderType = OrderByType.Descending, string search = "")
        {
            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return HandleReadException();
            }

            if (RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return await CollectionItemDetails(id, collection);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {

                var dataTask = Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, search, orderColumn, orderType == OrderByType.Ascending ? true : false, page - 1, pageSize);

                var totalTask = Service.GetByIdWithPagedCollectionPropertyCountAsync(cts.Token, id, collection, search);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var result = dataTask.Result;

                Type collectionItemType = RelationshipHelper.GetCollectionExpressionType(collection, typeof(TDto));
                object list = RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), result);

                var total = totalTask.Result;

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

                ViewBag.Collection = collection;
                ViewBag.Id = id;

                //For the time being collection properties are read only. DDD states that only the Aggregate Root should get updated.
                ViewBag.DisableCreate = true;
                ViewBag.DisableEdit = true;
                ViewBag.DisableDelete = true;
                ViewBag.DisableSorting = false;
                ViewBag.DisableEntityEvents = true;
                ViewBag.DisableSearch = false;

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("List", response);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        private async Task<ActionResult> CollectionItemDetails(string id, string collection)
        {
            if (!RelationshipHelper.IsValidCollectionExpression(collection, typeof(TDto)))
            {
                return HandleReadException();
            }

            if (!RelationshipHelper.IsCollectionExpressionCollectionItem(collection))
            {
                return HandleReadException();
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            Object data = null;
            try
            {
                var response = await Service.GetByIdWithPagedCollectionPropertyAsync(cts.Token, id, collection, "", null, false, null, null);

                var collectionItem = RelationshipHelper.GetCollectionExpressionData(collection, typeof(TDto), response);

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

            ViewBag.DisableEdit = true;
            ViewBag.Collection = RelationshipHelper.GetCollectionExpressionWithoutCurrentCollectionItem(collection);
            ViewBag.Id = id;

            return View("Details", data);
        }
        #endregion

    }
}

