using AutoMapper;
using DND.Domain.Constants;
using DND.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
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
    [Route("bucket-list")]
    public class BucketListController : BaseController
	{
        private readonly IBlogService _blogService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepositoryFactory;


        public BucketListController(IBlogService blogService, IMapper mapper, IFileSystemRepositoryFactory fileSystemRepositoryFactory)
             : base(mapper)
        {
            _blogService = blogService;
            _fileSystemRepositoryFactory = fileSystemRepositoryFactory;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<ActionResult> Index(int page = 1, int pageSize = 100, string orderColumn = nameof(FileInfo.LastWriteTime), string orderType = OrderByType.Descending)
		{
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());
           
            try
            {
                var repository = _fileSystemRepositoryFactory.CreateFileRepository(cts.Token, Server.GetWwwFolderPhysicalPathById(Folders.BucketList), true,"*.*", ".jpg",".jpeg", ".txt",".mp4");
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
		
	}
}