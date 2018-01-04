using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Email;
using Solution.Base.Helpers;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Interfaces.Repository;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

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
    public abstract class BaseFolderMetadataReadOnlyControllerAuthorize : BaseController
    {   
        public IFileSystemRepositoryFactory FileSystemRepositoryFactory { get; private set; }
        public Boolean Admin { get; set; }
        public Boolean IncludeSubDirectories { get; set; }
        public String PhysicalPath { get; set; }

        public BaseFolderMetadataReadOnlyControllerAuthorize(string physicalPath, Boolean includeSubDirectories, Boolean admin, IFileSystemRepositoryFactory fileSystemRepositoryFactory, IMapper mapper = null, IEmailService emailService = null)
        : base(mapper, emailService)
        {
            PhysicalPath = physicalPath;
            IncludeSubDirectories = includeSubDirectories;
            Admin = admin;
            FileSystemRepositoryFactory = fileSystemRepositoryFactory;
        }

        // GET: Default
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(DirectoryInfo.LastWriteTime), string orderType = OrderByType.Descending,string search = "")
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
                  
            try
            {
                var repository = FileSystemRepositoryFactory.CreateFolderRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                var dataTask = repository.SearchAsync(search, null, LamdaHelper.GetOrderByFunc<DirectoryInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
                var totalTask = repository.GetSearchCountAsync(search, null);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var rows = data.ToList().Select(Mapper.Map<DirectoryInfo, FolderMetadataDTO>).ToList();

                foreach (FolderMetadataDTO dto in rows)
                {
                    dto.Id = dto.Id.Replace(PhysicalPath, "");
                }

                var response = new WebApiPagedResponseDTO<FolderMetadataDTO>
                {
                    Page = page,
                    PageSize = pageSize,
                    Records = total,
                    Rows = rows,
                    OrderColumn = orderColumn,
                    OrderType = orderType,
                    Search = search
                };

                ViewBag.Search = search;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                ViewBag.DisableCreate = true;
                ViewBag.DisableSorting = true;
                ViewBag.DisableDelete = false;

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
        [Route("details/{*id}")]
        public virtual async Task<ActionResult> Details(string id)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
            DirectoryInfo data = null;
            try
            {
                var repository = FileSystemRepositoryFactory.CreateFolderRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);

                data = await repository.GetByPathAsync(id.Replace("/","\\"));

                if (data == null)
                    return HandleReadException();
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;

            var dto = Mapper.Map<FolderMetadataDTO>(data);
            dto.Id = dto.Id.Replace(PhysicalPath, "");

            return View("Details", dto);
        }

    }
}

