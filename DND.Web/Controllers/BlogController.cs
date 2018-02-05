using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Web.Models.BlogPostViewModels;
using Microsoft.AspNetCore.Mvc;
using Solution.Base.Controllers;
using Solution.Base.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Web.Controllers
{
    [Route("blog/[action]")]
    public class BlogController : BaseController
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService, IMapper mapper)
            : base(mapper)
        {
            _blogService = blogService;
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("")]
        public async Task<IActionResult> Posts(int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostService.GetPostsAsync(page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostService.GetTotalPostsAsync(true, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts
            };

            ViewBag.PageTitle = "Latest Posts";
            return View("PostList", blogPostListViewModel);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("archive/{year}/{month}/{title}")]
        public async Task<IActionResult> Post(int year, int month, string title)
        {
            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var post = await _blogService.BlogPostService.GetPostAsync(year, month, title, cts.Token);

            if (post == null)
                return NotFound();

            if (post.Published == false && User.Identity.IsAuthenticated == false)
                return Unauthorized();

            return View("Post", post);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("author/{authorSlug}")]
        public async Task<IActionResult> Author(string authorSlug, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostService.GetPostsForAuthorAsync(authorSlug, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostService.GetTotalPostsForAuthorAsync(authorSlug, cts.Token);
            var authorTask = _blogService.AuthorService.GetAuthorAsync(authorSlug, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask, authorTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;
            var author = authorTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Author = author
            };

            if (blogPostListViewModel.Author == null)
                return NotFound();

            ViewBag.PageTitle = String.Format(@"Latest posts for Author ""{0}""", blogPostListViewModel.Author.Name);

            return View("PostList", blogPostListViewModel);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("category/{categorySlug}")]
        public async Task<IActionResult> Category(string categorySlug, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostService.GetPostsForCategoryAsync(categorySlug, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostService.GetTotalPostsForCategoryAsync(categorySlug, cts.Token);
            var categoryTask = _blogService.CategoryService.GetCategoryAsync(categorySlug, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask, categoryTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;
            var category = categoryTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Category = category
            };

            if (blogPostListViewModel.Category == null)
                return NotFound();

            ViewBag.PageTitle = String.Format(@"Latest posts on category ""{0}""", blogPostListViewModel.Category.Name);

            return View("PostList", blogPostListViewModel);
        }

        [ResponseCache(CacheProfileName = "Cache24HourParams")]
        [Route("tag/{tagSlug}")]
        public async Task<IActionResult> Tag(string tagSlug, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostService.GetPostsForTagAsync(tagSlug, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostService.GetTotalPostsForTagAsync(tagSlug, cts.Token);
            var tagDTOTask = _blogService.TagService.GetTagAsync(tagSlug, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask, tagDTOTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;
            var tagDTO = tagDTOTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Tag = tagDTO
            };

            if (blogPostListViewModel.Tag == null)
                return NotFound();

            ViewBag.PageTitle = String.Format(@"Latest posts tagged on ""{0}""", blogPostListViewModel.Tag.Name);

            return View("PostList", blogPostListViewModel);
        }

        [Route("~/search")]
        public async Task<ViewResult> Search(string s, int page = 1)
        {
            int pageSize = 10;

            var cts = TaskHelper.CreateChildCancellationTokenSource(ClientDisconnectedToken());

            var postsTask = _blogService.BlogPostService.GetPostsForSearchAsync(s, page - 1, pageSize, cts.Token);
            var totalPostsTask = _blogService.BlogPostService.GetTotalPostsForSearchAsync(s, cts.Token);

            await TaskHelper.WhenAllOrException(cts, postsTask, totalPostsTask);

            var posts = postsTask.Result;
            var totalPosts = totalPostsTask.Result;

            var blogPostListViewModel = new BlogPostListViewModel
            {
                Page = page,
                PageSize = pageSize,
                Posts = posts.ToList(),
                TotalPosts = totalPosts,
                Search = s
            };

            ViewBag.PageTitle = String.Format(@"Lists of posts found for search text ""{0}""", s);

            return View("PostList", blogPostListViewModel);
        }
    }
}
