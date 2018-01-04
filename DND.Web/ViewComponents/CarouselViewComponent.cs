using DND.Domain.Constants;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using DND.Web.Models.CarouselViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Repository;
using Solution.Base.ModelMetadataCustom.DisplayAttributes;
using Solution.Base.ViewComponents;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    [ViewComponent]
    public class CarouselViewComponent : BaseViewComponent
    {
        private readonly IBlogService _blogService;
        private readonly ICarouselItemService _carouselItemService;
        private readonly IFileSystemRepositoryFactory _fileSystemRepository;


        public CarouselViewComponent(IBlogService blogService, ICarouselItemService carouselItemService, IFileSystemRepositoryFactory fileSystemRepository)
        {
            _fileSystemRepository = fileSystemRepository;
            _blogService = blogService;
            _carouselItemService = carouselItemService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            string orderColumn = nameof(CarouselItemDTO.DateCreated);
            string orderType = OrderByType.Descending;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            IEnumerable<BlogPostDTO> posts = null;
            IList<CarouselItemDTO> carouselItemsFinal = new List<CarouselItemDTO>();
            IEnumerable<CarouselItemDTO> carouselItems = null;

            IList<DirectoryInfo> albums = new List<DirectoryInfo>();
            IList<CarouselItemDTO> albumCarouselItems = new List<CarouselItemDTO>();


            var postsTask = _blogService.BlogPostService.GetPostsForCarouselAsync(0, 3, cts.Token);
            var carouselItemsTask = _carouselItemService.GetAsync(cts.Token, c => c.Published, LamdaHelper.GetOrderBy<CarouselItemDTO>(orderColumn, orderType), null, null);

            await TaskHelper.WhenAllOrException(cts, postsTask, carouselItemsTask);

            posts = postsTask.Result;
            carouselItems = carouselItemsTask.Result;

            var repository = _fileSystemRepository.CreateFolderRepositoryReadOnly(cts.Token, Server.GetWwwFolderPhysicalPathById(Folders.Gallery));
            foreach (CarouselItemDTO item in carouselItems)
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
