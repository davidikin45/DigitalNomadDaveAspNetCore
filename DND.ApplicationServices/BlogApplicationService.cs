using AutoMapper;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.ApplicationServices;
using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.ApplicationServices
{
    public class BlogApplicationService : BaseApplicationService, IBlogApplicationService
    {
        public IBlogPostApplicationService BlogPostApplicationService { get; private set; }
        public ICategoryApplicationService CategoryApplicationService { get; private set; }
        public ITagApplicationService TagApplicationService { get; private set; }
        public IAuthorApplicationService AuthorApplicationService { get; private set; }

        public BlogApplicationService(IMapper mapper,
            IBlogPostApplicationService blogPostApplicationService,
            ICategoryApplicationService categoryApplicationService,
            ITagApplicationService tagApplicationService,
            IAuthorApplicationService authorApplicationService)
            : base(mapper)
        {
            BlogPostApplicationService = blogPostApplicationService;
            CategoryApplicationService = categoryApplicationService;
            TagApplicationService = tagApplicationService;
            AuthorApplicationService = authorApplicationService;
        }

    }

    public class BlogPostApplicationService : BaseEntityApplicationService<IApplicationDbContext, BlogPost, BlogPostDTO>, IBlogPostApplicationService
    {
        protected virtual IBlogPostDomainService BlogPostDomainService { get; }

        public BlogPostApplicationService(IBlogPostDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {

        }

        public async Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken)
        {
            return await BlogPostDomainService.GetTotalPostsAsync(checkIsPublished, cancellationToken);
        }

        public IEnumerable<BlogPostDTO> GetPosts(int pageNo, int pageSize)
        {
            var posts = BlogPostDomainService.GetPosts(pageNo, pageSize);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsAsync(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsAsyncWithLocation(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsForCarouselAsync(pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsForAuthorAsync(authorSlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            return await BlogPostDomainService.GetTotalPostsForAuthorAsync(authorSlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsForCategoryAsync(categorySlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            return await BlogPostDomainService.GetTotalPostsForCategoryAsync(categorySlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsForTagAsync(tagSlug, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            return await BlogPostDomainService.GetTotalPostsForTagAsync(tagSlug, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            var posts = await BlogPostDomainService.GetPostsForSearchAsync(search, pageNo, pageSize, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            return await BlogPostDomainService.GetTotalPostsForSearchAsync(search, cancellationToken);
        }

        public async Task<BlogPostDTO> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            var bo = await BlogPostDomainService.GetPostAsync(year, month, titleSlug, cancellationToken);
            return Mapper.Map<BlogPostDTO>(bo);
        }

        #region "Admin"
        public override async Task<IEnumerable<BlogPostDTO>> SearchAsync(CancellationToken cancellationToken, string search = "", Expression<Func<BlogPostDTO, bool>> filter = null, Expression<Func<IQueryable<BlogPostDTO>, IOrderedQueryable<BlogPostDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPostDTO, object>>[] includeProperties)
        {
            var filterConverted = GetMappedSelector<BlogPostDTO, BlogPost, bool>(filter);
            var orderByConverted = GetMappedOrderBy<BlogPostDTO, BlogPost>(orderBy);
            var includesConverted = GetMappedIncludes<BlogPostDTO, BlogPost>(includeProperties);
            var list = includesConverted.ToList();
            list.Add(p => p.Tags.Select(t => t.Tag));
            includesConverted = list.ToArray();

            var entityList = await BlogPostDomainService.SearchAsync(cancellationToken, search, filterConverted, orderByConverted, pageNo, pageSize, includesConverted);

            IEnumerable<BlogPostDTO> dtoList = entityList.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return dtoList;
        }

        public async override Task<IEnumerable<BlogPostDTO>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<BlogPostDTO>, IOrderedQueryable<BlogPostDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPostDTO, object>>[] includeProperties)
        {
            return await GetPostsAsync(pageNo.Value, pageSize.Value, orderBy, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsAsync(int pageNo, int pageSize, Expression<Func<IQueryable<BlogPostDTO>, IOrderedQueryable<BlogPostDTO>>> orderBy, CancellationToken cancellationToken)
        {
            var mappedOrderBy = GetMappedOrderBy<BlogPostDTO, BlogPost>(orderBy);

            var posts = await BlogPostDomainService.GetPostsAsync(pageNo, pageSize, mappedOrderBy, cancellationToken);

            IEnumerable<BlogPostDTO> list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);

            return list;
        }

        public async override Task<BlogPostDTO> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetPostAsync(int.Parse(id.ToString()), cancellationToken);
        }

        public async Task<BlogPostDTO> GetPostAsync(int id, CancellationToken cancellationToken)
        {
            var bo = await BlogPostDomainService.GetPostAsync(id, cancellationToken);
            return Mapper.Map<BlogPostDTO>(bo);
        }

        public async override Task UpdateAsync(BlogPostDTO dto, CancellationToken cancellationToken)
        {
            var persistedPost = await BlogPostDomainService.GetFirstAsync(cancellationToken, p => p.Id == dto.Id, null, p => p.Tags, p => p.Locations);
            var persistedTags = persistedPost.Tags.ToList();
            var persistedLocations = persistedPost.Locations.ToList();
            Mapper.Map(dto, persistedPost);

            var insertTags = persistedPost.Tags.Except(persistedTags);
            var deleteTags = persistedTags.Except(persistedPost.Tags);

            var insertLocations = persistedPost.Locations.Except(persistedLocations);
            var deleteLocations = persistedLocations.Except(persistedPost.Locations);

            await BlogPostDomainService.UpdateAsync(persistedPost, insertTags, deleteTags, insertLocations, deleteLocations, cancellationToken);
        }
        #endregion
    }

    public class CategoryApplicationService : BaseEntityApplicationService<IApplicationDbContext, Category, CategoryDTO>, ICategoryApplicationService
    {

        protected virtual ICategoryDomainService CategoryDomainService { get; }

        public CategoryApplicationService(ICategoryDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {
            CategoryDomainService = domainService;
        }

        public async Task<CategoryDTO> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            var bo = await CategoryDomainService.GetCategoryAsync(categorySlug, cancellationToken);
            return Mapper.Map<CategoryDTO>(bo);
        }

        public async override Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<CategoryDTO>, IOrderedQueryable<CategoryDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<CategoryDTO, object>>[] includeProperties)
        {
            var categories = await CategoryDomainService.GetAllAsync(cancellationToken);
            return categories.ToList().Select(Mapper.Map<Category, CategoryDTO>);         
        }
    }

    public class TagApplicationService : BaseEntityApplicationService<IApplicationDbContext, Tag, TagDTO>, ITagApplicationService
    {

        protected virtual ITagDomainService TagDomainService { get; }

        public TagApplicationService(ITagDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {
            TagDomainService = domainService;
        }

        public async Task<TagDTO> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            var bo = await TagDomainService.GetTagAsync(tagSlug, cancellationToken);
            return Mapper.Map<TagDTO>(bo);
        }

        public async override Task<IEnumerable<TagDTO>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<TagDTO>, IOrderedQueryable<TagDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<TagDTO, object>>[] includeProperties)
        {
            var tags = await TagDomainService.GetAllAsync(cancellationToken);
            return tags.ToList().Select(Mapper.Map<Tag, TagDTO>);
        }
    }

    public class AuthorApplicationService : BaseEntityApplicationService<IApplicationDbContext, Author, AuthorDTO>, IAuthorApplicationService
    {
        protected virtual IAuthorDomainService AuthorDomainService { get; }

        public AuthorApplicationService(IAuthorDomainService domainService, IMapper mapper)
        : base(domainService, mapper)
        {
            AuthorDomainService = domainService;
        }

        public async Task<AuthorDTO> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            var bo = await AuthorDomainService.GetAuthorAsync(authorSlug, cancellationToken);
            return Mapper.Map<AuthorDTO>(bo);
        }

    }
}