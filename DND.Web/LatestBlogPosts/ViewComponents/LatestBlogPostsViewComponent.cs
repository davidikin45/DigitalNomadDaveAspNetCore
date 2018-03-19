using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;
using DND.Common.Helpers;
using DND.Common.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
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

            IEnumerable<BlogPostDTO> posts = postsTask.Result;

            return View(posts);
        }

    }
}
