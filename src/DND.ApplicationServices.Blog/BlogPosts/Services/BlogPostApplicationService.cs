using AutoMapper;
using DND.Common.ApplicationServices.SignalR;
using DND.Common.Implementation.ApplicationServices;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Interfaces.Blog.ApplicationServices;
using DND.Interfaces.Blog.DomainServices;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.BlogPosts.Services
{
    public class BlogPostApplicationService : ApplicationServiceEntityBase<BlogPost, BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDeleteDto, IBlogPostDomainService>, IBlogPostApplicationService
    {
        public BlogPostApplicationService(IBlogPostDomainService domainService, IMapper mapper, IHubContext<ApiNotificationHub<BlogPostDto>> hubContext)
        : base(domainService, mapper, hubContext)
        {

        }

        public override void AddIncludes(List<Expression<Func<BlogPost, object>>> includes)
        {
        
        }

        public async Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken)
        {
            return await DomainService.GetTotalPostsAsync(checkIsPublished, cancellationToken);
        }

        public IEnumerable<BlogPostDto> GetPosts(int pageNo, int pageSize)
        {
            var posts = DomainService.GetPosts(pageNo, pageSize);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsAsync(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsAsyncWithLocation(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsForCarouselAsync(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsForAuthorAsync(authorSlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            return await DomainService.GetTotalPostsForAuthorAsync(authorSlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsForCategoryAsync(categorySlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            return await DomainService.GetTotalPostsForCategoryAsync(categorySlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsForTagAsync(tagSlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            return await DomainService.GetTotalPostsForTagAsync(tagSlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await DomainService.GetPostsForSearchAsync(search, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            return await DomainService.GetTotalPostsForSearchAsync(search, cancellationToken);
        }

        public async Task<BlogPostDto> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetPostAsync(year, month, titleSlug, cancellationToken);
            return Mapper.Map<BlogPostDto>(bo);
        }

        #region "Admin"
        //public async override Task<Result> UpdateAsync(Object id, BlogPostDto dto, string updatedBy, CancellationToken cancellationToken)
        //{
        //    var persistedPost = await DomainService.GetFirstAsync(cancellationToken, p => p.Id == dto.Id, null, true);
        //    var persistedTags = persistedPost.Tags.ToList();
        //    var persistedLocations = persistedPost.Locations.ToList();
        //    Mapper.Map(dto, persistedPost);

        //    var insertTags = persistedPost.Tags.Except(persistedTags);
        //    var deleteTags = persistedTags.Except(persistedPost.Tags);

        //    var insertLocations = persistedPost.Locations.Except(persistedLocations);
        //    var deleteLocations = persistedLocations.Except(persistedPost.Locations);

        //    return await DomainService.UpdateAsync(persistedPost, insertTags, deleteTags, insertLocations, deleteLocations, updatedBy, cancellationToken);
        //}
        #endregion
    }
}