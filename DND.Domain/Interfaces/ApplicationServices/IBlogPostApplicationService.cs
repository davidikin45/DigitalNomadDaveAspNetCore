using DND.Domain.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IBlogPostApplicationService : IBaseEntityApplicationService<BlogPostDTO>
    {
        Task<BlogPostDTO> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken);

        IEnumerable<BlogPostDTO> GetPosts(int pageNo, int pageSize);
        Task<IEnumerable<BlogPostDTO>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPostDTO>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDTO>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDTO>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPostDTO>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPostDTO>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPostDTO>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken);

        //Admin
        Task<IEnumerable<BlogPostDTO>> GetPostsAsync(int pageNo, int pageSize, Expression<Func<IQueryable<BlogPostDTO>, IOrderedQueryable<BlogPostDTO>>> orderBy, CancellationToken cancellationToken);
        Task<BlogPostDTO> GetPostAsync(int id, CancellationToken cancellationToken);
    }
}
