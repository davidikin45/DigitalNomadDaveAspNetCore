﻿using AutoMapper;
using DND.Common.Controllers;
using DND.Common.Dtos;
using DND.Common.Filters;
using DND.Common.Helpers;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Email;
using DND.Common.Infrastructure.Settings;
using DND.Common.Interfaces.Repository;
using DND.Infrastructure.Constants;
using DND.Interfaces.Blog.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Mvc.Gallery.Controllers
{
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Gallery" })]
    [Route("gallery")]
    public class GalleryController : MvcControllerBase
    {
        private readonly IBlogApplicationService _blogService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;


        public GalleryController(IBlogApplicationService blogService, IMapper mapper, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, IEmailService emailService, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
            _blogService = blogService;
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 20, string orderColumn = nameof(DirectoryInfo.LastWriteTime), string orderType = "desc", string search = "")
		{
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var repository = _fileSystemGenericRepositoryFactory.CreateFolderRepository(cts.Token, Server.GetWwwFolderPhysicalPathById(Folders.Gallery));
                var dataTask = repository.SearchAsync(search, null, LamdaHelper.GetOrderByFunc<DirectoryInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
                var totalTask = repository.GetSearchCountAsync(search, null);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDto<DirectoryInfo>
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

                return View(response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        [NoAjaxRequest]
        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("{name}")]
        public virtual async Task<ActionResult> Gallery(string name, int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            try
            {
                if (name == "instagram")
                    return View("Instagram");

                string physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Gallery) + name;

                if (!System.IO.Directory.Exists(physicalPath))
                {
                    name = name.ToLower().Replace("-", " ");
                    physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Gallery) + name;
                }

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetGalleryViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                ViewBag.Album = new DirectoryInfo(Server.GetWwwFolderPhysicalPathById(Folders.Gallery) + name);

                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.OrderColumn = orderColumn;
                ViewBag.OrderType = orderType;

                return View(response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        [AjaxRequest]
        [ActionName("Gallery")]
        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("{name}")]
        public virtual async Task<ActionResult> GalleryList(string name, int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            try
            {
                string physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Gallery) + name;

                if (!System.IO.Directory.Exists(physicalPath))
                {
                    name = name.ToLower().Replace("-", " ");
                    physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Gallery) + name;
                }

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetGalleryViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                return PartialView("_GalleryAjax",response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        private async Task<WebApiPagedResponseDto<FileInfo>> GetGalleryViewModel(string physicalPath, int page = 1, int pageSize = 40, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = _fileSystemGenericRepositoryFactory.CreateFileRepository(cts.Token, physicalPath, true, "*.*", ".jpg", ".jpeg", ".mp4",".avi", ".txt");
            var dataTask = repository.GetAllAsync(LamdaHelper.GetOrderByFunc<FileInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
            var totalTask = repository.GetCountAsync(null);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var response = new WebApiPagedResponseDto<FileInfo>
            {
                Page = page,
                PageSize = pageSize,
                Records = total,
                Rows = data.ToList(),
                OrderColumn = orderColumn,
                OrderType = orderType
            };

            return response;
        }

    }
}