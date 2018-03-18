using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Alerts;
using DND.Common.Email;
using DND.Common.Helpers;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Models;
using DND.Common.Interfaces.Services;
using System;
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
    public abstract class BaseEntityController<TDto, IEntityService> : BaseEntityReadOnlyController<TDto, IEntityService>
        where TDto : class, IBaseEntity
        where IEntityService : IBaseEntityApplicationService<TDto>
    {
        public BaseEntityController(Boolean admin, IEntityService service, IMapper mapper = null, IEmailService emailService = null)
        : base(admin, service, mapper,emailService)
        {
        }

        // GET: Default/Create
        [Route("create")]
        public virtual ActionResult Create()
        {
            var instance = CreateNewDtoInstance();
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Create", instance);
        }

        // POST: Default/Create
        [HttpPost]
        [Route("create")]
        public virtual async Task<ActionResult> Create(TDto dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    await Service.CreateAsync(dto, cts.Token);
                    return RedirectToControllerDefault().WithSuccess(this, Messages.AddSuccessful);
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

        // GET: Default/Edit/5
        [Route("edit/{id}")]
        public virtual async Task<ActionResult> Edit(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TDto data = null;
            try
            {
                data = await Service.GetByIdAsync(id, cts.Token);
                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Edit", data);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        // POST: Default/Edit/5
        [HttpPost]
        [Route("edit/{id}")]
        public virtual async Task<ActionResult> Edit(string id, TDto dto)
        {
            //dto.Id = id;
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    await Service.UpdateAsync(dto, cts.Token);
                    return RedirectToControllerDefault().WithSuccess(this, Messages.UpdateSuccessful);
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

        // GET: Default/Delete/5
        [Route("delete/{id}")]
        public virtual async Task<ActionResult> Delete(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            TDto data = null;
            try
            {             
               data = await Service.GetByIdAsync(id, cts.Token);
                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Delete", data);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }           
        }

        // POST: Default/Delete/5
        [HttpPost, ActionName("Delete"), Route("delete/{id}")]
        public virtual async Task<ActionResult> DeleteConfirmed(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    await Service.DeleteAsync(id, cts.Token);
                    return RedirectToControllerDefault().WithSuccess(this, Messages.DeleteSuccessful);
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

    }
}

