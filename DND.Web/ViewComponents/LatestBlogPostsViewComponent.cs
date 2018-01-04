using DND.Domain.Constants;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Services;
using DND.Web.Models.SidebarViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Helpers;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.ViewComponents
{
    public class LatestblogPostsViewComponent : ViewComponent
    {
        private readonly IBlogService _blogService;

        public LatestblogPostsViewComponent(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(HttpContext.RequestAborted);

            var postsTask = _blogService.BlogPostService.GetPostsAsync(0, 6, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask);

            IEnumerable<BlogPostDTO> posts = postsTask.Result;

            return View(posts);
        }

    }
}
