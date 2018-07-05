using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Alerts;
using DND.Common.Email;
using DND.Common.Helpers;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Services;
using System.Threading.Tasks;
using DND.Common.Interfaces.Dtos;
using System.Collections.Generic;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

        public BaseEntityWebApiControllerAuthorize(IEntityService service, IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, ITypeHelperService typeHelperService = null, IConfiguration configuration = null)
        : base(service, mapper, emailService, urlHelper, typeHelperService, configuration)
        {

        }

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
            if(result.IsFailure)
            {
                return ValidationErrors(result);
            }
            var createdDto = result.Value;

            //return CreatedAtRoute("", new { id = createdDto.Id }, createdDto);

            //return ApiSuccessMessage(Messages.AddSuccessful, createdDto.Id);
            //return Success(createdDto);

            return CreatedAtAction(nameof(Create), new { id = createdDto.Id }, createdDto);
        }

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
            if(dto is IBaseDtoWithId)
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

            var result = await Service.UpdateAsync(id, dto, Username, cts.Token);
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }
            //return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
            //return Success(dto);
            return NoContent();
        }

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

            var dto = await Service.GetUpdateDtoByIdAsync(id, cts.Token);

            if (dto == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            var ops = new List<Operation<TUpdateDto>>();
            foreach (var op in dtoPatch.Operations)
            {
                ops.Add(new Operation<TUpdateDto>(op.op, op.path, op.from, op.value));
            }
       
            var dtoPatchTypes = new JsonPatchDocument<TUpdateDto>(ops, dtoPatch.ContractResolver);

            dtoPatchTypes.ApplyTo(dto, ModelState);

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            var result = await Service.UpdateAsync(id, dto, Username, cts.Token);
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }

            //return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
            //return Success(dto);
            return NoContent();
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        //[Route("{id}")]
        //[HttpDelete]
        //////[HttpPost]
        //[ProducesResponseType(typeof(WebApiMessage), 200)]
        //public virtual async Task<IActionResult> Delete(string id)
        //{
        //    var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

        //    //if (!(await Service.ExistsAsync(cts.Token, id)))
        //    //{
        //    //    return ApiNotFoundErrorMessage(Messages.NotFound);
        //    //}

        //    var result = await Service.DeleteAsync(id, cts.Token);
        //    if (result.IsFailure)
        //    {
        //        return ValidationErrors(result);
        //    }
        //    //return ApiSuccessMessage(Messages.DeleteSuccessful, id);
        //    return NoContent();
        //}

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

            //if (!(await Service.ExistsAsync(cts.Token, dto.Id)))
            //{
            //    return ApiNotFoundErrorMessage(Messages.NotFound);
            //}

            if (dto == null || id.ToString() != dto.Id.ToString())
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var result = await Service.DeleteAsync(dto, Username, cts.Token); // // This should give concurrency checking
            if (result.IsFailure)
            {
                return ValidationErrors(result);
            }

            //return ApiSuccessMessage(Messages.DeleteSuccessful, id);
            return NoContent();
        }

    }
}

