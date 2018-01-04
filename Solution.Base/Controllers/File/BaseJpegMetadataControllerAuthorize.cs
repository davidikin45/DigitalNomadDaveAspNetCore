using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Alerts;
using Solution.Base.Email;
using Solution.Base.Helpers;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Implementation.Models;
using Solution.Base.Interfaces.Repository;
using System;
using System.Threading.Tasks;

namespace Solution.Base.Controllers
{

    //Edit returns a view of the resource being edited, the Update updates the resource it self

    //C - Create - POST
    //R - Read - GET
    //U - Update - PUT
    //D - Delete - DELETE

    //If there is an attribute applied(via[HttpGet], [HttpPost], [HttpPut], [AcceptVerbs], etc), the action will accept the specified HTTP method(s).
    //If the name of the controller action starts the words "Get", "Post", "Put", "Delete", "Patch", "Options", or "Head", use the corresponding HTTP method.
    //Otherwise, the action supports the POST method.
    [Authorize(Roles = "admin")]
    public abstract class BaseJpegMetadataControllerAuthorize : BaseJpegMetadataReadOnlyControllerAuthorize
    {
        public BaseJpegMetadataControllerAuthorize(string physicalPath, Boolean includeSubDirectories, Boolean admin, IFileSystemRepositoryFactory fileSystemRepositoryFactory, IMapper mapper = null, IEmailService emailService = null)
        : base(physicalPath, includeSubDirectories, admin, fileSystemRepositoryFactory, mapper, emailService)
        {
        }

        // GET: Default/Edit/5
        [Route("edit/{*id}")]
        public virtual async Task<ActionResult> Edit(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            JpegMetadata data = null;
            try
            {
                var repository = FileSystemRepositoryFactory.CreateJpegMetadataRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                data = await repository.MetadataGetByPathAsync(id.Replace("/", "\\"));

                var dto = Mapper.Map<JpegMetadataDTO>(data);

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Edit", dto);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        // POST: Default/Edit/5
        [HttpPost]
        [Route("edit/{*id}")]
        public virtual async Task<ActionResult> Edit(string id, JpegMetadataDTO dto)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var metadata = new JpegMetadata(PhysicalPath + id.Replace("/", "\\"));
                    Mapper.Map(dto, metadata);

                    var lat = metadata.Latitude;

                    metadata.SaveWithCaption(dto.Caption, dto.DateCreated);

                    //await Service.UpdateAsync(dto, cts.Token);
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
        [Route("delete/{*id}")]
        public virtual async Task<ActionResult> Delete(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            JpegMetadata data = null;
            try
            {

                var repository = FileSystemRepositoryFactory.CreateJpegMetadataRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                data = await repository.MetadataGetByPathAsync(id.Replace("/", "\\"));

                var dto = Mapper.Map<JpegMetadataDTO>(data);

                ViewBag.PageTitle = Title;
                ViewBag.Admin = Admin;
                return View("Delete", dto);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        // POST: Default/Delete/5
        [HttpPost, ActionName("Delete"), Route("delete/{*id}")]
        public virtual async Task<ActionResult> DeleteConfirmed(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            if (ModelState.IsValid)
            {
                try
                {
                    var repository = FileSystemRepositoryFactory.CreateJpegMetadataRepository(cts.Token, PhysicalPath, IncludeSubDirectories);
                    repository.Delete(id.Replace("/", "\\"));

                    return RedirectToControllerDefault().WithSuccess(this, Messages.DeleteSuccessful);
                }
                catch (Exception ex)
                {
                    HandleUpdateException(ex);
                }
            }
            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;
            return View("Delete", id);
        }
    }
}

