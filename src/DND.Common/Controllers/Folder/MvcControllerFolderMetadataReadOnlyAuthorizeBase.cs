﻿using AutoMapper;
using DND.Common.Dtos;
using DND.Common.Helpers;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
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
    [Authorize(Roles = "admin")]
    public abstract class MvcControllerFolderMetadataReadOnlyAuthorizeBase : MvcControllerBase
    {   
        public IFileSystemGenericRepositoryFactory FileSystemGenericRepositoryFactory { get; private set; }
        public Boolean Admin { get; set; }
        public Boolean IncludeSubDirectories { get; set; }
        public String PhysicalPath { get; set; }

        public MvcControllerFolderMetadataReadOnlyAuthorizeBase(string physicalPath, Boolean includeSubDirectories, Boolean admin, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IMapper mapper = null, IEmailService emailService = null, AppSettings appSettings = null)
        : base(mapper, emailService, appSettings)
        {
            PhysicalPath = physicalPath;
            IncludeSubDirectories = includeSubDirectories;
            Admin = admin;
            FileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        // GET: Default
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(DirectoryInfo.LastWriteTime), string orderType = "desc",string search = "")
        {

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
                  
            try
            {
                var repository = FileSystemGenericRepositoryFactory.CreateFolderRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);
                var dataTask = repository.SearchAsync(search, null, LamdaHelper.GetOrderByFunc<DirectoryInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
                var totalTask = repository.GetSearchCountAsync(search, null);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var rows = data.ToList().Select(Mapper.Map<DirectoryInfo, FolderMetadataDto>).ToList();

                foreach (FolderMetadataDto dto in rows)
                {
                    dto.Id = dto.Id.Replace(PhysicalPath, "");
                }

                var response = new WebApiPagedResponseDto<FolderMetadataDto>
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
            catch
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
                var repository = FileSystemGenericRepositoryFactory.CreateFolderRepositoryReadOnly(cts.Token, PhysicalPath, IncludeSubDirectories);

                data = await repository.GetByPathAsync(id.Replace("/","\\"));

                if (data == null)
                    return HandleReadException();
            }
            catch
            {
                return HandleReadException();
            }

            ViewBag.PageTitle = Title;
            ViewBag.Admin = Admin;

            var dto = Mapper.Map<FolderMetadataDto>(data);
            dto.Id = dto.Id.Replace(PhysicalPath, "");

            return View("Details", dto);
        }

    }
}

