using DND.Common.Helpers;
using DND.Common.Infrastructure;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.Implementation.Blog.ViewComponents
{
    public class LatestblogPostsViewComponent : ViewComponent
    {
        private readonly IBlogApplicationService _blogService;

        public LatestblogPostsViewComponent(IBlogApplicationService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(HttpContext.RequestAborted);

            var postsTask = _blogService.BlogPostApplicationService.GetPostsAsync(0, 6, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask);

            IEnumerable<BlogPostDto> posts = postsTask.Result;

            return View(posts);
        }

    }
}
