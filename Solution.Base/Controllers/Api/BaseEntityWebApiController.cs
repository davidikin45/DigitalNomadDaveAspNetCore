﻿using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Alerts;
using Solution.Base.Email;
using Solution.Base.Helpers;
using Solution.Base.Interfaces.Models;
using Solution.Base.Interfaces.Services;
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
    public abstract class BaseEntityWebApiController<TDto, IEntityService> : BaseEntityReadOnlyWebApiController<TDto, IEntityService>
        where TDto : class, IBaseEntity
        where IEntityService : IBaseEntityService<TDto>
    {   

        public BaseEntityWebApiController(IEntityService service, IMapper mapper = null, IEmailService emailService = null)
        : base(service, mapper, emailService)
        {
          
        }

        [Route("add")]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Add(TDto dto)
        {
            if(dto == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var createdDto = await Service.CreateAsync(dto, cts.Token);
            return ApiCreatedSuccessMessage(Messages.AddSuccessful, createdDto.Id);

            //return CreatedAtRoute("", new { id = createdDto.Id }, createdDto);
        }

        [Route("update")]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Update(TDto dto)
        {
            if (dto == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            await Service.UpdateAsync(dto, cts.Token);
            return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
        }

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> UpdatePartial(object id, JsonPatchDocument<TDto> dtoPatch)
        {
            if (dtoPatch == null)
            {
                return ApiErrorMessage(Messages.RequestInvalid);
            }

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var dto = await Service.GetByIdAsync(id, cts.Token);

            if (dto == null)
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            dtoPatch.ApplyTo(dto);

            await Service.UpdateAsync(dto, cts.Token);
           
            return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
        }

        [Route("delete")]
        [HttpPost]
        [ProducesResponseType(typeof(WebApiMessage),200)]
        public virtual async Task<IActionResult> Delete([FromBody]int id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (!(await Service.ExistsAsync(cts.Token, id)))
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            await Service.DeleteAsync(id, cts.Token);
            return ApiSuccessMessage(Messages.DeleteSuccessful, id);
        }

    }
}
