using AutoMapper;
using DND.Common.Alerts;
using DND.Common.Controllers.Api;
using DND.Common.DomainEvents;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.Helpers;
using DND.Common.Implementation.Data;
using DND.Common.Implementation.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    //[Authorize(Roles = "admin")]
    public abstract class BaseEntityControllerAuthorize<TCreateDto, TReadDto, TUpdateDto, TDeleteDto, IEntityService> : BaseEntityReadOnlyControllerAuthorize<TReadDto, IEntityService>
        where TCreateDto : class, IBaseDto
        where TReadDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
        where TUpdateDto : class, IBaseDto, IBaseDtoConcurrencyAware
        where TDeleteDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
        where IEntityService : IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
    {
        private ActionEvents actionEvents;
        public BaseEntityControllerAuthorize(Boolean admin, IEntityService service, IMapper mapper = null, IEmailService emailService = null, IConfiguration configuration = null)
        : base(admin, service, mapper, emailService, configuration)
        {
            actionEvents = new ActionEvents();
        }

        #region New Instance
        [Authorize(Policy = ApiScopes.Create)]
        [Route("new")]
        public virtual ActionResult Create()
        {
            var instance = Service.GetCreateDefaultDto();
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Create", instance);
        }
        #endregion

        #region Create
        [Authorize(Policy = ApiScopes.Create)]
        [HttpPost]
        [Route("new")]
        public virtual async Task<ActionResult> Create(TCreateDto dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await Service.CreateAsync(dto, Username, cts.Token);
                    if (result.IsFailure)
                    {
                        HandleUpdateException(result, null, true);
                    }
                    else
                    {
                        return RedirectToControllerDefault().WithSuccess(this, Messages.AddSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            //error
            return View("Create", dto);
        }
        #endregion

        #region Get for Edit
        [Authorize(Policy = ApiScopes.Update)]
        [Route("edit/{id}")]
        public virtual async Task<ActionResult> Edit(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TUpdateDto data = null;
            try
            {
                data = await Service.GetUpdateDtoByIdAsync(id, cts.Token);
                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Edit", data);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }
        #endregion

        #region Update
        [Authorize(Policy = ApiScopes.Update)]
        [HttpPost]
        [Route("edit/{id}")]
        public virtual async Task<ActionResult> Edit(string id, TUpdateDto dto)
        {
            //dto.Id = id;
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await Service.UpdateGraphAsync(id, dto, Username, cts.Token);
                    if (result.IsFailure)
                    {
                        HandleUpdateException(result, dto, true);
                    }
                    else
                    {
                        return RedirectToControllerDefault().WithSuccess(this, Messages.UpdateSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Edit", dto);
        }
        #endregion

        #region Get for Delete
        [Authorize(Policy = ApiScopes.Delete)]
        [Route("delete/{id}")]
        public virtual async Task<ActionResult> Delete(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TDeleteDto data = null;
            try
            {
                data = await Service.GetDeleteDtoByIdAsync(id, cts.Token);
                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Delete", data);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }
        #endregion

        #region Delete
        [Authorize(Policy = ApiScopes.Delete)]
        [HttpPost, ActionName("Delete"), Route("delete/{id}")]
        public virtual async Task<ActionResult> DeleteConfirmed(string id, TDeleteDto dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    //var result = await Service.DeleteAsync(id, cts.Token);
                    var result = await Service.DeleteAsync(dto, Username, cts.Token); // This should give concurrency checking
                    if (result.IsFailure)
                    {
                        HandleUpdateException(result, dto, true);
                    }
                    else
                    {
                        return RedirectToControllerDefault().WithSuccess(this, Messages.DeleteSuccessful);
                    }
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            var data = await Service.GetByIdAsync(id, cts.Token);
            return View("Delete", data);
        }
        #endregion

        #region Create New Collection Item Instance
        [Authorize(Policy = ApiScopes.Write)]
        [HttpGet]
        [Route("new/{*collection}")]
        public virtual ActionResult CreateCollectionItem(string collection)
        {
            if (!RelationshipHelper.IsValidCollectionItemCreateExpression(collection, typeof(TUpdateDto)))
            {
                return HandleReadException();
            }

            ViewBag.Collection = collection.Replace("/", ".");
            ViewBag.CollectionIndex = Guid.NewGuid().ToString();

            var instance = Service.GetCreateDefaultCollectionItemDto(collection);

            return PartialView("_CreateCollectionItem", instance);
        }
        #endregion

        #region Trigger Actions
        [Authorize(Policy = ApiScopes.Update)]
        [HttpPost]
        [Route("{id}/trigger-action")]
        public virtual async Task<ActionResult> TriggerAction(string id, string action, IFormCollection collection)
        {
            if (string.IsNullOrWhiteSpace(action) || !actionEvents.IsValidAction<TUpdateDto>(action))
            {
                return HandleReadException();
            }

            dynamic args = null;
            if (collection != null)
            {
                args = collection.ToDynamic();
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var eventDto = new ActionDto()
                {
                    Action = action,
                    Args = args
                };

                var result = await Service.TriggerActionAsync(id, eventDto, Username, cts.Token);
                if (result.IsFailure)
                {
                    return HandleReadException();
                }
                else
                {
                    return RedirectToControllerDefault().WithSuccess(this, Messages.ActionSuccessful);
                }
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }
        #endregion
    }
}

