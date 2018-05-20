using DND.Domain.Models;
using DND.Common.Interfaces.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Domain.Blog.BlogPosts;
using DND.Common.Implementation.Validation;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface IBlogPostDomainService : IBaseEntityDomainService<BlogPost>
    {
        Task<BlogPost> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken);

        IEnumerable<BlogPost> GetPosts(int pageNo, int pageSize);
        Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<IEnumerable<BlogPost>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPost>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPost>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken);

        Task<IEnumerable<BlogPost>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPost>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken);


        Task<IEnumerable<BlogPost>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken);
        Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken);

        //Admin
        Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy, CancellationToken cancellationToken);
        Task<BlogPost> GetPostAsync(int id, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(BlogPost entity, IEnumerable<BlogPostTag> insertTags, IEnumerable<BlogPostTag> deleteTags, IEnumerable<BlogPostLocation> insertLocations, IEnumerable<BlogPostLocation> deleteLocations, string updatedBy, CancellationToken cancellationToken);
    }
}
