using AutoMapper;
using DND.Domain.Constants;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Controllers;
using DND.Common.Helpers;
using DND.Common.Implementation.Dtos;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;
using DND.Common.ModelMetadataCustom.DisplayAttributes;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Implementation.BucketList.Controllers
{
    [Route("bucket-list")]
    public class BucketListController : BaseController
	{
        private readonly IBlogApplicationService _blogService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepositoryFactory;


        public BucketListController(IBlogApplicationService blogService, IMapper mapper, IFileSystemRepositoryFactory fileSystemRepositoryFactory)
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

                var response = new WebApiPagedResponseDto<FileInfo>
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