using AutoMapper;
using DND.Domain.Interfaces.Services;
using DND.Domain.DTOs;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Extensions;
using Solution.Base.Implementation.Services;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Solution.Base.Infrastructure;

namespace DND.Services
{
    public class BlogService : BaseBusinessService, IBlogService
    {
        public IBlogPostService BlogPostService { get; private set; }
        public ICategoryService CategoryService { get; private set; }
        public ITagService TagService { get; private set; }
        public IAuthorService AuthorService { get; private set; }

        public BlogService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper,
            IBlogPostService blogPostService,
            ICategoryService categoryService, 
            ITagService tagService,
            IAuthorService authorService)
            : base(baseUnitOfWorkScopeFactory, mapper)
        {
            BlogPostService = blogPostService;
            CategoryService = categoryService;
            TagService = tagService;
            AuthorService = authorService;
        }   

    }

    public class BlogPostService : BaseEntityService<IApplicationDbContext, BlogPost, BlogPostDTO>, IBlogPostService
    {
        public BlogPostService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public async Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken)
        {
            int count = 0;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => !checkIsPublished || p.Published);
            }

            return count;
        }

        public IEnumerable<BlogPostDTO> GetPosts(int pageNo, int pageSize)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting))
            {
                var posts = UoW.Repository<IApplicationDbContext, BlogPost>().Get(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.ShowInCarousel, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize);
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Author.UrlSlug.Equals(authorSlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            int count = 0;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Author.UrlSlug.Equals(authorSlug));
            }

            return count;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Category.UrlSlug.Equals(categorySlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            int count = 0;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Category.UrlSlug.Equals(categorySlug));
            }

            return count;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug)), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }
            return list;
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            int count = 0;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug)));
            }

            return count;
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search)) || p.Locations.Any(l => l.Location.Name.Equals(search))), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }
            return list;
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            int count = 0;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                count = await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search))) || p.Locations.Any(l => l.Location.Name.Equals(search)));
            }

            return count;
        }

        public async Task<BlogPostDTO> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await UoW.Repository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.DateCreated.Year == year && p.DateCreated.Month == month && p.UrlSlug.Equals(titleSlug), null, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                return Mapper.Map<BlogPostDTO>(result);
            }
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

            IEnumerable<BlogPostDTO> dtoList;
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var entityList = await unitOfWork.Repository<IApplicationDbContext, BlogPost>().SearchAsync(search, filterConverted, orderByConverted, pageNo * pageSize, pageSize, includesConverted);
                dtoList = entityList.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
                return dtoList;
            }
        }

        public async override Task<IEnumerable<BlogPostDTO>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<BlogPostDTO>, IOrderedQueryable<BlogPostDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPostDTO, object>>[] includeProperties)
        {
            return await GetPostsAsync(pageNo.Value, pageSize.Value, orderBy, cancellationToken);
        }

        public async Task<IEnumerable<BlogPostDTO>> GetPostsAsync(int pageNo, int pageSize, Expression<Func<IQueryable<BlogPostDTO>, IOrderedQueryable<BlogPostDTO>>> orderBy, CancellationToken cancellationToken)
        {
            var mappedOrderBy = GetMappedOrderBy<BlogPostDTO, BlogPost>(orderBy);
            IEnumerable<BlogPostDTO> list;
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                IEnumerable<BlogPost> posts = null;
                if (orderBy != null)
                {
                    posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(null, mappedOrderBy, pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                }
                else
                {
                    posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(null, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                }
              
                list = posts.ToList().Select(Mapper.Map<BlogPost, BlogPostDTO>);
            }

            return list;
        }

        public async override Task<BlogPostDTO> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetPostAsync(int.Parse(id.ToString()), cancellationToken);
        }

        public async Task<BlogPostDTO> GetPostAsync(int id, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await UoW.Repository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.Id == id, null, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                return Mapper.Map<BlogPostDTO>(result);
            }
        }

        public async override Task<BlogPostDTO> CreateAsync(BlogPostDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Title);
            }

            return await base.CreateAsync(dto, cancellationToken);
        }

        public async override Task UpdateAsync(BlogPostDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Title);
            }

            using (var UoW = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var persistedPost = await UoW.Repository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.Id == dto.Id, null, p => p.Tags, p => p.Locations);
                var persistedTags = persistedPost.Tags.ToList();
                var persistedLocations = persistedPost.Locations.ToList();
                Mapper.Map(dto, persistedPost);

                var insertTags = persistedPost.Tags.Except(persistedTags);
                var deleteTags = persistedTags.Except(persistedPost.Tags);

                foreach (BlogPostTag tag in insertTags)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostTag>().Create(tag);
                }

                foreach (BlogPostTag tag in deleteTags)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostTag>().Delete(tag.Id);
                }

                var insertLocations = persistedPost.Locations.Except(persistedLocations);
                var deleteLocations = persistedLocations.Except(persistedPost.Locations);

                foreach (BlogPostLocation location in insertLocations)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostLocation>().Create(location);
                }

                foreach (BlogPostLocation location in deleteLocations)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostLocation>().Delete(location.Id);
                }

                UoW.Repository<IApplicationDbContext, BlogPost>().Update(persistedPost);

                await UoW.CompleteAsync(cancellationToken);
            }
        }
        #endregion
    }

    public class CategoryService : BaseEntityService<IApplicationDbContext, Category, CategoryDTO>, ICategoryService
    {
        public CategoryService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public async Task<CategoryDTO> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await UoW.Repository<IApplicationDbContext, Category>().GetFirstAsync(c => c.UrlSlug.Equals(categorySlug));
                return Mapper.Map<CategoryDTO>(result);
            }
        }

        public async override Task<IEnumerable<CategoryDTO>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<CategoryDTO>, IOrderedQueryable<CategoryDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<CategoryDTO, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var categories = await UoW.Repository<IApplicationDbContext, Category>().GetAllAsync(o => o.OrderBy(c => c.Name));
                return categories.ToList().Select(Mapper.Map<Category, CategoryDTO>);
            }
        }

        public async override Task<CategoryDTO> CreateAsync(CategoryDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return await base.CreateAsync(dto, cancellationToken);
        }
    }

    public class TagService : BaseEntityService<IApplicationDbContext, Tag, TagDTO>, ITagService
    {
        public TagService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public async Task<TagDTO> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await UoW.Repository<IApplicationDbContext, Tag>().GetFirstAsync(t => t.UrlSlug.Equals(tagSlug));
                return Mapper.Map<TagDTO>(result);
            }
        }

        public async override Task<IEnumerable<TagDTO>> GetAllAsync(CancellationToken cancellationToken, Expression<Func<IQueryable<TagDTO>, IOrderedQueryable<TagDTO>>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<TagDTO, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var tags = await UoW.Repository<IApplicationDbContext, Tag>().GetAllAsync(o => o.OrderBy(c => c.Name));
                return tags.ToList().Select(Mapper.Map<Tag, TagDTO>);
            }
        }

        public async override Task UpdateAsync(TagDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            await base.UpdateAsync(dto, cancellationToken);
        }

        public async override Task<TagDTO> CreateAsync(TagDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return await base.CreateAsync(dto, cancellationToken);
        }
    }

    public class AuthorService : BaseEntityService<IApplicationDbContext, Author, AuthorDTO>, IAuthorService
    {
        public AuthorService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory, IMapper mapper)
        : base(baseUnitOfWorkScopeFactory, mapper)
        {

        }

        public async Task<AuthorDTO> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                var result = await UoW.Repository<IApplicationDbContext, Author>().GetFirstAsync(c => c.UrlSlug.Equals(authorSlug));
                return Mapper.Map<AuthorDTO>(result);
            }
        }

        public async override Task UpdateAsync(AuthorDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            await base.UpdateAsync(dto, cancellationToken);
        }

        public async override Task<AuthorDTO> CreateAsync(AuthorDTO dto, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(dto.UrlSlug))
            {
                dto.UrlSlug = UrlSlugger.ToUrlSlug(dto.Name);
            }

            return await base.CreateAsync(dto, cancellationToken);
        }
    }
}