﻿using AutoMapper;
using DND.Common.Alerts;
using DND.Common.DomainEvents;
using DND.Common.Email;
using DND.Common.Extensions;
using DND.Common.Helpers;
using DND.Common.Implementation.Data;
using DND.Common.Implementation.DTOs;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Dtos;
using DND.Common.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Common.Controllers.Api
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    public abstract class BaseEntityWebApiControllerAuthorize<TCreateDto, TReadDto, TUpdateDto, TDeleteDto, IEntityService> : BaseEntityReadOnlyWebApiControllerAuthorize<TReadDto, IEntityService>
         where TCreateDto : class, IBaseDto
         where TReadDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
         where TUpdateDto : class, IBaseDto, IBaseDtoConcurrencyAware
         where TDeleteDto : class, IBaseDtoWithId, IBaseDtoConcurrencyAware
        where IEntityService : IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
    {
        private ActionEvents actionEvents;
        public BaseEntityWebApiControllerAuthorize(IEntityService service, IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, ITypeHelperService typeHelperService = null, IConfiguration configuration = null)
        : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {
            actionEvents = new ActionEvents();
        }

        #region New Instance
        [Route("new")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Create)]
        [HttpGet]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual IActionResult NewDefault()
        {
            var response = Service.GetCreateDefaultDto();

            var links = CreateLinksForCreate();

            var linkedResourceToReturn = response.ShapeData("")
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }
        #endregion

        #region Create
        //[Route("create")]
        /// <summary>
        /// Creates the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Create)]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Create([FromBody] TCreateDto dto)
        {
            if (dto == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var result = await Service.CreateAsync(dto, User.Identity.Name, cts.Token);
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }
            var createdDto = result.Value;

            //return CreatedAtRoute("", new { id = createdDto.Id }, createdDto);

            //return ApiSuccessMessage(Messages.AddSuccessful, createdDto.Id);
            //return Success(createdDto);

            return CreatedAtAction(nameof(Create), new { id = createdDto.Id }, createdDto);
        }
        #endregion

        #region Bulk Create
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Create)]
        [Route("bulk")]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> BulkCreate([FromBody] TCreateDto[] dtos)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var results = await Service.BulkCreateAsync(dtos, Username, cts.Token);

            return BulkCreateResponse(results);
        }
        #endregion

        #region Get for Edit

        /// <summary>
        /// Ges for edit.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [FormatFilter]
        [Route("edit/{id}"), Route("edit/{id}.{format}")]
        //[Route("get/{id}"), Route("get/{id}.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public virtual async Task<IActionResult> GeForEdit(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //By passing true we include the Composition properties which should be any child or join entities.
            var response = await Service.GetUpdateDtoByIdAsync(id, cts.Token);

            if (response == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var links = CreateLinks(id, "");

            var linkedResourceToReturn = response.ShapeData("")
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }
        #endregion

        #region Bulk Get for Edit
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [FormatFilter]
        [Route("edit/({ids})"), Route("edit/({ids}).{format}")]
        [Route("bulk/edit/({ids})"), Route("bulk/edit/({ids}).{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<object>), 200)]
        public virtual async Task<IActionResult> GetUpdateCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetUpdateDtosByIdsAsync(cts.Token, ids);

            var list = response.ToList();

            if (ids.Count() != list.Count())
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            return Success(list);
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [Route("{id}")]
        [HttpPut]
        //[HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Update(string id, [FromBody] TUpdateDto dto)
        {
            if (dto is IBaseDtoWithId)
            {
                IBaseDtoWithId dtoWithId = dto as IBaseDtoWithId;
                if (dto == null || id.ToString() != dtoWithId.Id.ToString())
                {
                    return ApiErrorMessage(Messages.RequestInvalid);
                }
            }

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //var exists = await Service.ExistsAsync(cts.Token, id);

            //if (!exists)
            //{
            //    return ApiNotFoundErrorMessage(Messages.NotFound);
            //}

            var result = await Service.UpdateGraphAsync(id, dto, Username, cts.Token);
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }
            //return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
            //return Success(dto);
            return NoContent();
        }
        #endregion

        #region Bulk Update
        /// <summary>
        /// Bulks the update.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [Route("bulk")]
        [HttpPut]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> BulkUpdate([FromBody] TUpdateDto[] dtos)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var results = await Service.BulkUpdateGraphAsync(dtos, Username, cts.Token);

            return BulkUpdateResponse(results);
        }
        #endregion

        #region Update Partial
        /// <summary>
        /// Updates the partial.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="dtoPatch">The dto patch.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> UpdatePartial(string id, [FromBody] JsonPatchDocument dtoPatch)
        {
            if (dtoPatch == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var result = await Service.UpdatePartialAsync(id, dtoPatch, Username, cts.Token);
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }

            return NoContent();
        }
        #endregion

        #region Bulk Partial Update
        /// <summary>
        /// Bulks the update.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [Route("bulk")]
        [HttpPatch]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> BulkPartialUpdate([FromBody] PatchDto[] dtos)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var results = await Service.BulkUpdatePartialAsync(dtos, Username, cts.Token);

            return BulkUpdateResponse(results);
        }
        #endregion

        #region Get for Delete
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Delete)]
        [FormatFilter]
        [Route("Delete/{id}"), Route("Delete/{id}.{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public virtual async Task<IActionResult> GetForDelete(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            //By passing true we include the Composition properties which should be any child or join entities.
            var response = await Service.GetDeleteDtoByIdAsync(id, cts.Token);

            if (response == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var links = CreateLinks(id, "");

            var linkedResourceToReturn = response.ShapeData("")
                as IDictionary<string, object>;

            linkedResourceToReturn.Add("links", links);

            return Ok(linkedResourceToReturn);
        }
        #endregion

        #region Bulk Get for Delete
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Delete)]
        [FormatFilter]
        [Route("delete/({ids})"), Route("delete/({ids}).{format}")]
        [Route("bulk/delete/({ids})"), Route("bulk/delete/({ids}).{format}")]
        [HttpGet]
        [ProducesResponseType(typeof(List<object>), 200)]
        public virtual async Task<IActionResult> GetDeleteCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var response = await Service.GetDeleteDtosByIdsAsync(cts.Token, ids);

            var list = response.ToList();

            if (ids.Count() != list.Count())
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            return Success(list);
        }
        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified dto.
        /// </summary>
        /// <param name="dto">The dto.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Delete)]
        [Route("{id}")]
        [HttpDelete]
        //[HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> DeleteDto(string id, [FromBody] TDeleteDto dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());


            if (dto == null || id.ToString() != dto.Id.ToString())
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var result = await Service.DeleteAsync(dto, Username, cts.Token); // // This should give concurrency checking
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }

            return NoContent();
        }
        #endregion

        #region Bulk Delete
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Delete)]
        [Route("bulk")]
        [HttpDelete]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> BulkDelete([FromBody] TDeleteDto[] dtos)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var results = await Service.BulkDeleteAsync(dtos, Username, cts.Token);

            return BulkDeleteResponse(results);
        }
        #endregion

        #region Create New Collection Item Instance
        [Route("new/{*collection}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Create)]
        [HttpGet]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual IActionResult NewCollectionItem(string collection)
        {
            if (!RelationshipHelper.IsValidCollectionItemCreateExpression(collection, typeof(TUpdateDto)))
            {
                return ApiErrorMessage(Messages.CollectionInvalid);
            }

            var response = Service.GetCreateDefaultCollectionItemDto(collection);

            return Ok(response);
        }
        #endregion

        #region Trigger Actions
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [Route("{id}/trigger-action")]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> TriggerAction(string id, [FromBody] ActionDto action)
        {
            if (action == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            if (string.IsNullOrWhiteSpace(action.Action) || !actionEvents.IsValidAction<TUpdateDto>(action.Action))
            {
                return ApiErrorMessage(Messages.ActionInvalid);
            }

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var result = await Service.TriggerActionAsync(id, action, Username, cts.Token);
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }

            return NoContent();
        }
        #endregion

        #region Bulk Trigger Action
        /// <summary>
        /// Triggers the actions.
        /// </summary>
        /// <param name="actions">The actions.</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = ApiScopes.Update)]
        [HttpPost]
        [Route("bulk/trigger-actions")]
        public virtual async Task<IActionResult> TriggerActions([FromBody] BulkActionDto[] actions)
        {
            if (actions == null || actions.Any(a => a.Id is null || string.IsNullOrWhiteSpace(a.Id.ToString())))
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            foreach (var action in actions)
            {
                if (action == null || string.IsNullOrWhiteSpace(action.Action) || !actionEvents.IsValidAction<TUpdateDto>(action.Action))
                {
                    return ApiErrorMessage(Messages.ActionsInvalid);
                }
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var results = await Service.TriggerActionsAsync(actions, Username, cts.Token);

            return BulkTriggerActionResponse(results);
        }
        #endregion

    }
}

