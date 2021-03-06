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
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Mvc.Videos.Controllers
{
    [TypeFilter(typeof(FeatureAuthFilter), Arguments = new object[] { "Videos" })]
    [Route("videos")]
    public class VideosController : MvcControllerBase
    {
        private readonly IFileSystemGenericRepositoryFactory _fileSystemGenericRepositoryFactory;


        public VideosController(IMapper mapper, IEmailService emailService, IFileSystemGenericRepositoryFactory fileSystemGenericRepositoryFactory, AppSettings appSettings)
             : base(mapper, emailService, appSettings)
        {
            _fileSystemGenericRepositoryFactory = fileSystemGenericRepositoryFactory;
        }

        [NoAjaxRequest]
        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public virtual async Task<ActionResult> Index(int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            try
            {

                string physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Videos);

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetVideosViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                ViewBag.Album = new DirectoryInfo(Server.GetWwwFolderPhysicalPathById(Folders.Videos));

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
        [ActionName("Index")]
        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public virtual async Task<ActionResult> IndexList(int page = 1, int pageSize = 10, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            try
            {              
                string physicalPath = Server.GetWwwFolderPhysicalPathById(Folders.Videos);

                if (!System.IO.Directory.Exists(physicalPath))
                    return HandleReadException();

                var response = await GetVideosViewModel(physicalPath, page, pageSize, orderColumn, orderType);

                return PartialView("_VideoAjax",response);
            }
            catch
            {
                return HandleReadException();
            }
        }

        private async Task<WebApiPagedResponseDto<FileInfo>> GetVideosViewModel(string physicalPath, int page = 1, int pageSize = 40, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = _fileSystemGenericRepositoryFactory.CreateFileRepository(cts.Token, physicalPath, true, "*.*", ".mp4",".avi", ".txt");
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