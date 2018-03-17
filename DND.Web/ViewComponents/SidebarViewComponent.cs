using DND.Domain.Constants;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Models.SidebarViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Repository;
using Solution.Base.ViewComponents;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    public class SidebarViewComponent : BaseViewComponent
    {
        private readonly IBlogApplicationService _blogService;
        private readonly IFileSystemRepositoryFactory FileSystemRepository;

        public SidebarViewComponent(IBlogApplicationService blogService, IFileSystemRepositoryFactory fileSystemRepository)
        {
            FileSystemRepository = fileSystemRepository;
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var repository = FileSystemRepository.CreateFileRepository(cts.Token, Server.GetWwwFolderPhysicalPathById(Folders.Gallery), true, "*.*", ".jpg", ".jpeg");

            IList<CategoryDTO> categories = null;
            IEnumerable<TagDTO> tags = null;
            IEnumerable<BlogPostDTO> posts = null;
            IEnumerable<FileInfo> photos = null;

            var categoriesTask = _blogService.CategoryApplicationService.GetAllAsync(cts.Token);
            var tagsTask = _blogService.TagApplicationService.GetAllAsync(cts.Token);
            var postsTask = _blogService.BlogPostApplicationService.GetPostsAsync(0, 10, cts.Token);
            var photosTask = repository.GetAllAsync(d => d.OrderByDescending(f => f.LastWriteTime), 0, 6);

            await TaskHelper.WhenAllOrException(cts, tagsTask, categoriesTask);

            List<Task<int>> countTasks = new List<Task<int>>();

            //foreach (TagDTO dto in tagsTask.Result)
            //{
            //    tagCountTasks.Add(_blogService.BlogPostService.GetTotalPostsForTagAsync(dto.UrlSlug, cts.Token).ContinueWith(t => dto.Count = t.Result));
            //}
            categories = categoriesTask.Result.ToList();
            foreach (CategoryDTO dto in categories)
            {
                countTasks.Add(_blogService.BlogPostApplicationService.GetTotalPostsForCategoryAsync(dto.UrlSlug, cts.Token));
            }

            await TaskHelper.WhenAllOrException(cts, categoriesTask, postsTask, photosTask);
            await TaskHelper.WhenAllOrException(cts, countTasks.ToArray());

            int i = 0;
            foreach (CategoryDTO dto in categories)
            {
                dto.Count = countTasks[i].Result;
                i++;
            }

            tags = tagsTask.Result;
            posts = postsTask.Result;
            photos = photosTask.Result;

            var widgetViewModel = new BlogWidgetViewModel
            {
                Categories = categories,
                Tags = tags.ToList(),
                LatestPosts = posts.ToList(),
                LatestPhotos = photos.ToList()
            };

            return View(widgetViewModel);
        }

    }
}
