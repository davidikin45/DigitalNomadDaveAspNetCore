using AutoMapper;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.Validation;
using DND.Domain.Blog.BlogPosts;
using DND.Domain.Blog.BlogPosts.Dtos;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices.Blog.BlogPosts.Services
{
    public class BlogPostApplicationService : BaseEntityApplicationService<IApplicationDbContext, BlogPost, BlogPostDto, BlogPostDto, BlogPostDto, BlogPostDto, IBlogPostDomainService>, IBlogPostApplicationService
    {
        public BlogPostApplicationService(IBlogPostDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
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
        public override async Task<IEnumerable<BlogPostDto>> SearchAsync(CancellationToken cancellationToken, string search = "", Expression<Func<BlogPostDto, bool>> filter = null, Expression<Func<IQueryable<BlogPostDto>, IOrderedQueryable<BlogPostDto>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPostDto, object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<BlogPostDto, BlogPost, bool>(filter);
            var orderByConverted = GetMappedOrderBy<BlogPostDto, BlogPost>(orderBy);
            var includesConverted = GetMappedIncludes<BlogPostDto, BlogPost>(includeProperties);
            var list = includesConverted.ToList();
            list.Add(p => p.Tags.Select(t => t.Tag));
            includesConverted = list.ToArray();

            var entityList = await DomainService.SearchAsync(cancellationToken, search, filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

            IEnumerable<BlogPostDto> dtoList = entityList.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return dtoList;
        }

        public async override Task<IEnumerable<BlogPostDto>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<BlogPostDto>, IOrderedQueryable<BlogPostDto>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPostDto, object>>[] includeProperties)
        {
            return await GetPostsAsync(pageNo.Value, pageSize.Value, orderBy, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDto>> GetPostsAsync(int pageNo, int pageSize, Expression<Func<IQueryable<BlogPostDto>, IOrderedQueryable<BlogPostDto>>> orderBy, CancellationToken cancellationToken)
        {
            var mappedOrderBy = GetMappedOrderBy<BlogPostDto, BlogPost>(orderBy);

            var posts = await DomainService.GetPostsAsync(pageNo, pageSize, mappedOrderBy, cancellationToken);

            IEnumerable<BlogPostDto> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDto>);

            return list;
        }

        public async override Task<BlogPostDto> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetPostAsync(int.Parse(id.ToString()), cancellationToken);
        }

        public async Task<BlogPostDto> GetPostAsync(int id, CancellationToken cancellationToken)
        {
            var bo = await DomainService.GetPostAsync(id, cancellationToken);
            return Mapper.Map<BlogPostDto>(bo);
        }

        public async override Task<Result> UpdateAsync(Object id, BlogPostDto dto, CancellationToken cancellationToken)
        {
            var persistedPost = await DomainService.GetFirstAsync(cancellationToken, p => p.Id == dto.Id, null, p => p.Tags, p => p.Locations);
            var persistedTags = persistedPost.Tags.ToList();
            var persistedLocations = persistedPost.Locations.ToList();
            Mapper.Map(dto, persistedPost);

            var insertTags = persistedPost.Tags.Except(persistedTags);
            var deleteTags = persistedTags.Except(persistedPost.Tags);

            var insertLocations = persistedPost.Locations.Except(persistedLocations);
            var deleteLocations = persistedLocations.Except(persistedPost.Locations);

            return await DomainService.UpdateAsync(persistedPost, insertTags, deleteTags, insertLocations, deleteLocations, cancellationToken);
        }
        #endregion
    }
}