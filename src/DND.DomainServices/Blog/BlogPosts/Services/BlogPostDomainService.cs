using DND.Domain.Interfaces.DomainServices;
using DND.Domain.Interfaces.Persistance;
using DND.Domain.Models;
using DND.Common.Implementation.ApplicationServices;
using DND.Common.Implementation.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Interfaces.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using DND.Domain.Blog.BlogPosts;
using DND.Common.Implementation.Validation;
using System.ComponentModel.DataAnnotations;
using DND.Common.Enums;

namespace DND.DomainServices.Blog.BlogPosts.Services
{
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
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetCountAsync(p => !checkIsPublished || p.Published).ConfigureAwait(false);
            }

        }

        public IEnumerable<BlogPost> GetPosts(int pageNo, int pageSize)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting))
            {
                return UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().Get(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location));
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.ShowInCarousel, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Author.UrlSlug.Equals(authorSlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Author.UrlSlug.Equals(authorSlug)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Category.UrlSlug.Equals(categorySlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Category.UrlSlug.Equals(categorySlug)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug)), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug))).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search)) || p.Locations.Any(l => l.Location.Name.Equals(search))), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetCountAsync(p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search))) || p.Locations.Any(l => l.Location.Name.Equals(search))).ConfigureAwait(false);
            }
        }

        public async Task<BlogPost> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.DateCreated.Year == year && p.DateCreated.Month == month && p.UrlSlug.Equals(titleSlug), null, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        #region "Admin"
        public override async Task<IEnumerable<BlogPost>> SearchAsync(CancellationToken cancellationToken, string search = "", Expression<Func<BlogPost, bool>> filter = null, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPost, object>>[] includeProperties)
        {
            using (var unitOfWork = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await unitOfWork.ReadOnlyRepository<IApplicationDbContext, BlogPost>().SearchAsync(search, filter, orderBy, pageNo * pageSize, pageSize, includeProperties).ConfigureAwait(false);
            }
        }

        public async override Task<IEnumerable<BlogPost>> GetAllAsync(CancellationToken cancellationToken, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy = null, int? pageNo = default(int?), int? pageSize = default(int?), params Expression<Func<BlogPost, object>>[] includeProperties)
        {
            return await GetPostsAsync(pageNo.Value, pageSize.Value, orderBy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, Func<IQueryable<BlogPost>, IOrderedQueryable<BlogPost>> orderBy, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                IEnumerable<BlogPost> posts = null;
                if (orderBy != null)
                {
                    posts = await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(null, orderBy, pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
                }
                else
                {
                    posts = await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetAsync(null, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
                }

                return posts;
            }
        }

        public async override Task<BlogPost> GetByIdAsync(object id, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await GetPostAsync(int.Parse(id.ToString()), cancellationToken).ConfigureAwait(false);
        }

        public async Task<BlogPost> GetPostAsync(int id, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(BaseUnitOfWorkScopeOption.JoinExisting, cancellationToken))
            {
                return await UoW.ReadOnlyRepository<IApplicationDbContext, BlogPost>().GetFirstAsync(p => p.Id == id, null, p => p.Category, p => p.Author, p => p.Tags.Select(t => t.Tag), p => p.Locations.Select(t => t.Location)).ConfigureAwait(false);
            }
        }

        public async override Task<Result<BlogPost>> CreateAsync(BlogPost entity, string createdBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Title);
            }

            return await base.CreateAsync(entity, createdBy, cancellationToken).ConfigureAwait(false);
        }

        public async Task<Result> UpdateAsync(BlogPost entity, IEnumerable<BlogPostTag> insertTags, IEnumerable<BlogPostTag> deleteTags, IEnumerable<BlogPostLocation> insertLocations, IEnumerable<BlogPostLocation> deleteLocations, string updatedBy, CancellationToken cancellationToken)
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

                await UoW.CompleteAsync(cancellationToken).ConfigureAwait(false);
            }

            return Result.Ok();
        }
        #endregion

        public async override Task<IEnumerable<ValidationResult>> DbDependantValidateAsync(BlogPost entity, ValidationMode mode)
        {
            var errors = new List<ValidationResult>();

            return errors;
        }
    }
}