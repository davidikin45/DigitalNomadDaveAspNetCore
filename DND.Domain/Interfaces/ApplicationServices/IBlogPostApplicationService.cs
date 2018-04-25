using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.Blog.BlogPosts.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IBlogPostApplicationService : IBaseEntityApplicationService<BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto>
    {
        Task<BlogPostDto> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken);

        IEnumerable<BlogPostDto> GetPosts(int pageNo, int pageSize);
        Task<IEnumerable<BlogPostDto>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPostDto>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDto>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDto>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDto>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPostDto>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPostDto>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken);

        //Admin
        Task<IEnumerable<BlogPostDto>> GetPostsAsync(int pageNo, int pageSize, Expression<Func<IQueryable<BlogPostDto>, IOrderedQueryable<BlogPostDto>>> orderBy, CancellationToken cancellationToken);
        Task<BlogPostDto> GetPostAsync(int id, CancellationToken cancellationToken);
    }
}
