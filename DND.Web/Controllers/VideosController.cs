using AutoMapper;
using DND.Domain.Constants;
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
    [Route("videos")]
    public class VideosController : BaseController
	{
        private readonly IFileSystemRepositoryFactory _fileSystemRepositoryFactory;


        public VideosController(IMapper mapper, IFileSystemRepositoryFactory fileSystemRepositoryFactory)
             : base(mapper)
        {
            _fileSystemRepositoryFactory = fileSystemRepositoryFactory;
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
            catch (Exception ex)
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

                return PartialView("_GalleryList",response);
            }
            catch (Exception ex)
            {
                return HandleReadException();
            }
        }

        private async Task<WebApiPagedResponseDTO<FileInfo>> GetVideosViewModel(string physicalPath, int page = 1, int pageSize = 40, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = _fileSystemRepositoryFactory.CreateFileRepository(cts.Token, physicalPath, true, "*.*", ".mp4", ".txt");
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