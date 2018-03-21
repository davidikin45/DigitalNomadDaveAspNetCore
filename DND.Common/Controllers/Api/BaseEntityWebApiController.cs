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
    public abstract class BaseEntityWebApiController<TCreateDto, TReadDto, TUpdateDto, TDeleteDto, IEntityService> : BaseEntityReadOnlyWebApiController<TReadDto, IEntityService>
         where TCreateDto : class, IBaseDto
         where TReadDto : class, IBaseDtoWithId
         where TUpdateDto : class, IBaseDto
         where TDeleteDto : class, IBaseDtoWithId
        where IEntityService : IBaseEntityApplicationService<TCreateDto, TReadDto, TUpdateDto, TDeleteDto>
    {

        public BaseEntityWebApiController(IEntityService service, IMapper mapper = null, IEmailService emailService = null, IUrlHelper urlHelper = null, ITypeHelperService typeHelperService = null)
        : base(service, mapper, emailService, urlHelper, typeHelperService)
        {

        }

        //[Route("create")]
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

            var createdDto = await Service.CreateAsync(dto, cts.Token);
            //return CreatedAtRoute("", new { id = createdDto.Id }, createdDto);

            //return ApiSuccessMessage(Messages.AddSuccessful, createdDto.Id);
            //return Success(createdDto);

            return CreatedAtAction(nameof(Create), new { id = createdDto.Id }, createdDto);
        }

        [Route("{id}")]
        [HttpPut]
        //[HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Update(object id, [FromBody] TUpdateDto dto)
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

            await Service.UpdateAsync(id, dto, cts.Token);
            //return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
            //return Success(dto);
            return NoContent();
        }

        [Route("{id}")]
        [HttpPatch]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> UpdatePartial(object id, [FromBody] JsonPatchDocument<TUpdateDto> dtoPatch)
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

            dtoPatch.ApplyTo(dto, ModelState);

            TryValidateModel(dto);

            if (!ModelState.IsValid)
            {
                return ValidationErrors(ModelState);
            }

            await Service.UpdateAsync(id, dto, cts.Token);

            //return ApiSuccessMessage(Messages.UpdateSuccessful, dto.Id);
            //return Success(dto);
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        //[HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Delete(int id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (!(await Service.ExistsAsync(cts.Token, id)))
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            await Service.DeleteAsync(id, cts.Token);
            //return ApiSuccessMessage(Messages.DeleteSuccessful, id);
            return NoContent();
        }

        [HttpDelete]
        //[HttpPost]
        [ProducesResponseType(typeof(WebApiMessage), 200)]
        public virtual async Task<IActionResult> Delete([FromBody] TDeleteDto dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (!(await Service.ExistsAsync(cts.Token, dto.Id)))
            {
                return ApiNotFoundErrorMessage(Messages.NotFound);
            }

            await Service.DeleteAsync(dto, cts.Token);
            //return ApiSuccessMessage(Messages.DeleteSuccessful, id);
            return NoContent();
        }

    }
}

