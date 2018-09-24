using DND.Common.Helpers;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;
using DND.Common.ViewComponents;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.CMS.CarouselItems.Dtos;
using DND.Infrastructure.Constants;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.CMS.ApplicationServices;
using DND.Web.Mvc.CarouselItem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Mvc.CarouselItem.ViewComponents
{
    [ViewComponent]
    public class CarouselViewComponent : BaseViewComponent
    {
        private readonly IBlogApplicationService _blogService;
        private readonly ICarouselItemApplicationService _carouselItemService;
        private readonly IFileSystemGenericRepositoryFactory _fileSystemRepository;


        public CarouselViewComponent(IBlogApplicationService blogService, ICarouselItemApplicationService carouselItemService, IFileSystemGenericRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _blogService = blogService;
            _carouselItemService = carouselItemService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(CarouselItemDto.DateCreated);
            string orderType = OrderByType.Descending;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<BlogPostDto> posts = null;
            IList<CarouselItemDto> carouselItemsFinal = new List<CarouselItemDto>();
            IEnumerable<CarouselItemDto> carouselItems = null;

            IList<DirectoryInfo> albums = new List<DirectoryInfo>();
            IList<CarouselItemDto> albumCarouselItems = new List<CarouselItemDto>();


            var postsTask = _blogService.BlogPostApplicationService.GetPostsForCarouselAsync(0, 3, cts.Token);
            var carouselItemsTask = _carouselItemService.GetAsync(cts.Token, c => c.Published, LamdaHelper.GetOrderBy<CarouselItemDto>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, postsTask, carouselItemsTask);

            posts = postsTask.Result;
            carouselItems = carouselItemsTask.Result;

            var repository = _fileSystemRepository.CreateFolderRepositoryReadOnly(cts.Token, Server.GetWwwFolderPhysicalPathById(Folders.Gallery));
            foreach (CarouselItemDto item in carouselItems)
            {
                if (!string.IsNullOrEmpty(item.Album))
                {
                    var album = repository.GetByPath(item.Album);
                    if (album != null)
                    {
                        albums.Add(album);
                        albumCarouselItems.Add(item);
                    }
                }
                else
                {
                    carouselItemsFinal.Add(item);
                }
            }

            var carouselViewModel = new CarouselViewModel
            {
                Posts = posts.ToList(),

                Albums = albums.ToList(),
                AlbumCarouselItems = albumCarouselItems.ToList(),

                CarouselItems = carouselItemsFinal.ToList(),
                ItemCount = posts.Count() + albums.Count() + carouselItemsFinal.Count()
            };

            return View(carouselViewModel);
        }

    }
}
