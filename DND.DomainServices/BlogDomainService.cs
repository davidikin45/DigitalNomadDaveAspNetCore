using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using Solution.Base.Implementation.ApplicationServices;
using Solution.Base.Implementation.DomainServices;
using Solution.Base.Infrastructure;
using Solution.Base.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices
{
    public class BlogDomainService : BaseDomainService, IBlogDomainService
    {
        public IBlogPostDomainService BlogPostDomainService { get; private set; }
        public ICategoryDomainService CategoryDomainService { get; private set; }
        public ITagDomainService TagDomainService { get; private set; }
        public IAuthorDomainService AuthorDomainService { get; private set; }

        public BlogDomainService(
            IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory,
            IBlogPostDomainService blogPostDomainService,
            ICategoryDomainService categoryDomainService,
            ITagDomainService tagDomainService,
            IAuthorDomainService authorDomainService)
            : base(baseUnitOfWorkScopeFactory)
        {
            BlogPostDomainService = blogPostDomainService;
            CategoryDomainService = categoryDomainService;
            TagDomainService = tagDomainService;
            AuthorDomainService = authorDomainService;
        }   

    }

    public class BlogPostDomainService : BaseEntityDomainService<IApplicationDbContext, BlogPost>, IBlogPostDomainService
    {
        public BlogPostDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => !checkIsPublished || p.Published);
            }

        }

        public IEnumerable<BlogPost> GetPosts(int pageNo, int pageSize)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting))
            {
                return UoW.Repository<IApplicationDbContext, BlogPost>().Get(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.ShowInCarousel, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Author.UrlSlug.Equals(authorSlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Author.UrlSlug.Equals(authorSlug));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Category.UrlSlug.Equals(categorySlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Category.UrlSlug.Equals(categorySlug));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug)), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug)));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search)) || p.Locations.Any(l => l.Location.Name.Equals(search))), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
              return await UoW.Repository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search))) || p.Locations.Any(l => l.Location.Name.Equals(search)));
            }
        }

        public async Task<BlogPost> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.DateCreated.Year == year && p.DateCreated.Month == month && p.UrlSlug.Equals(titleSlug), null, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        #region "Admin"
        public override async Task<IEnumerable<BlogPost>> SearchAsync(CancellationToken cancellationToken, string search = "", Expression<Func<BlogPost, bool>> filter = null, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPost, object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
               return await unitOfWork.Repository<IApplicationDbContext, BlogPost>().SearchAsync(search, filter, orderBy, pageNo * pageSize, pageSize, includeProperties);
            }
        }

        public async override Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPost, object>>[] includeProperties)
        {
            return await GetPostsAsync(pageNo.Value, pageSize.Value, orderBy, cancellationToken);
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                IEnumerable<BlogPost> posts = null;
                if (orderBy != null)
                {
                    posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(null, orderBy, pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                }
                else
                {
                    posts = await UoW.Repository<IApplicationDbContext, BlogPost>().GetAsync(null, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
                }

                return posts;
            }
        }

        public async override Task<BlogPost> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetPostAsync(int.Parse(id.ToString()), cancellationToken);
        }

        public async Task<BlogPost> GetPostAsync(int id, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.Id == id, null, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async override Task<BlogPost> CreateAsync(BlogPost entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Title);
            }

            return await base.CreateAsync(entity, cancellationToken);
        }

        public async Task UpdateAsync(BlogPost entity, IEnumerable<BlogPostTag> insertTags, IEnumerable<BlogPostTag> deleteTags, IEnumerable<BlogPostLocation> insertLocations, IEnumerable<BlogPostLocation> deleteLocations, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Title);
            }

            using (var UoW = UnitOfWorkFactory.Create(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {

                foreach (BlogPostTag tag in insertTags)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostTag>().Create(tag);
                }

                foreach (BlogPostTag tag in deleteTags)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostTag>().Delete(tag.Id);
                }

                foreach (BlogPostLocation location in insertLocations)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostLocation>().Create(location);
                }

                foreach (BlogPostLocation location in deleteLocations)
                {
                    UoW.Repository<IApplicationDbContext, BlogPostLocation>().Delete(location.Id);
                }

                UoW.Repository<IApplicationDbContext, BlogPost>().Update(entity);

                await UoW.CompleteAsync(cancellationToken);
            }
        }
        #endregion
    }

    public class CategoryDomainService : BaseEntityDomainService<IApplicationDbContext, Category>, ICategoryDomainService
    {
        public CategoryDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Category>().GetFirstAsync(c => c.UrlSlug.Equals(categorySlug));
            }
        }

        public async override Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<Category, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Category>().GetAllAsync(o => o.OrderBy(c => c.Name));
            }
        }

        public async override Task<Category> CreateAsync(Category entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, cancellationToken);
        }
    }

    public class TagDomainService : BaseEntityDomainService<IApplicationDbContext, Tag>, ITagDomainService
    {
        public TagDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Tag> GetTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Tag>().GetFirstAsync(t => t.UrlSlug.Equals(tagSlug));
            }
        }

        public async override Task<IEnumerable<Tag>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<Tag>, IOrderedQueryable<Tag>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<Tag, object>>[] includeProperties)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Tag>().GetAllAsync(o => o.OrderBy(c => c.Name));
            }
        }

        public async override Task UpdateAsync(Tag entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            await base.UpdateAsync(entity, cancellationToken);
        }

        public async override Task<Tag> CreateAsync(Tag entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, cancellationToken);
        }
    }

    public class AuthorDomainService : BaseEntityDomainService<IApplicationDbContext, Author>, IAuthorDomainService
    {
        public AuthorDomainService(IBaseUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public async Task<Author> GetAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.Repository<IApplicationDbContext, Author>().GetFirstAsync(c => c.UrlSlug.Equals(authorSlug));
            }
        }

        public async override Task UpdateAsync(Author entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            await base.UpdateAsync(entity, cancellationToken);
        }

        public async override Task<Author> CreateAsync(Author entity, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Name);
            }

            return await base.CreateAsync(entity, cancellationToken);
        }
    }
}