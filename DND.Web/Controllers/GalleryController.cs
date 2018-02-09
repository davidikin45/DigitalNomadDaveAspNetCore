﻿using AutoMapper;
using DND.Domain.Constants;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Filters;
using Solution.Base.Helpers;
using Solution.Base.Implementation.DTOs;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Repository;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Controllers
{
    [Route("gallery")]
    public class GalleryController : BaseController
	{
        private readonly IBlogService _blogService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepositoryFactory;


        public GalleryController(IBlogService blogService, IMapper mapper, IFileSystemRepositoryFactory fileSystemRepositoryFactory)
             : base(mapper)
        {
            _blogService = blogService;
            _fileSystemRepositoryFactory = fileSystemRepositoryFactory;
        }

        //[ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 20, string orderColumn = nameof(DirectoryInfo.LastWriteTime), string orderType = OrderByType.Descending, string search = "")
		{
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            try
            {
                var repository = _fileSystemRepositoryFactory.CreateFolderRepository(cts.Token, Server.GetWwwFolderPhysicalPathById(Folders.Gallery));
                var dataTask = repository.SearchAsync(search, null, LamdaHelper.GetOrderByFunc<DirectoryInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
                var totalTask = repository.GetSearchCountAsync(search, null);

                await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

                var data = dataTask.Result;
                var total = totalTask.Result;

                var response = new WebApiPagedResponseDTO<DirectoryInfo>
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
            catch (Exception ex)
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
            catch (Exception ex)
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
                name = name.ToLower().Replace("-", " ");
                string physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Gallery) + name;

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetGalleryViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                return PartialView("_GalleryList",response);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        private async Task<WebApiPagedResponseDTO<FileInfo>> GetGalleryViewModel(string physicalPath, int page = 1, int pageSize = 40, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = _fileSystemRepositoryFactory.CreateFileRepository(cts.Token, physicalPath, true, "*.*", ".jpg", ".jpeg", ".mp4", ".txt");
            var dataTask = repository.GetAllAsync(LamdaHelper.GetOrderByFunc<FileInfo>(orderColumn, orderType), (page - 1) * pageSize, pageSize);
            var totalTask = repository.GetCountAsync(null);

            await TaskHelper.WhenAllOrException(cts, dataTask, totalTask);

            var data = dataTask.Result;
            var total = totalTask.Result;

            var response = new WebApiPagedResponseDTO<FileInfo>
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