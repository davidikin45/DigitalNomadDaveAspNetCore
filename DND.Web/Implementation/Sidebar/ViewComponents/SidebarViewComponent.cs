using DND.Common.Helpers;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.Repository;
using DND.Common.ViewComponents;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Blog.Categories.Dtos;
using DND.Domain.Blog.Tags.Dtos;
using DND.Domain.Constants;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Web.Implementation.Sidebar.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.Implementation.Sidebar.ViewComponents
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

            IList<CategoryDto> categories = null;
            IEnumerable<TagDto> tags = null;
            IEnumerable<BlogPostDto> posts = null;
            IEnumerable<FileInfo> photos = null;

            var categoriesTask = _blogService.CategoryApplicationService.GetAllAsync(cts.Token);
            var tagsTask = _blogService.TagApplicationService.GetAllAsync(cts.Token);
            var postsTask = _blogService.BlogPostApplicationService.GetPostsAsync(0, 10, cts.Token);
            var photosTask = repository.GetAllAsync(d => d.OrderByDescending(f => f.LastWriteTime), 0, 6);

            await TaskHelper.WhenAllOrException(cts, tagsTask, categoriesTask);

            List<Task<int>> countTasks = new List<Task<int>>();

            //foreach (TagDto dto in tagsTask.Result)
            //{
            //    tagCountTasks.Add(_blogService.BlogPostService.GetTotalPostsForTagAsync(dto.UrlSlug, cts.Token).ContinueWith(t => dto.Count = t.Result));
            //}
            categories = categoriesTask.Result.ToList();
            foreach (CategoryDto dto in categories)
            {
                countTasks.Add(_blogService.BlogPostApplicationService.GetTotalPostsForCategoryAsync(dto.UrlSlug, cts.Token));
            }

            await TaskHelper.WhenAllOrException(cts, categoriesTask, postsTask, photosTask);
            await TaskHelper.WhenAllOrException(cts, countTasks.ToArray());

            int i = 0;
            foreach (CategoryDto dto in categories)
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
