using DND.Common.DomainServices;
using DND.Common.Infrastructure;
using DND.Common.Infrastructure.Interfaces.Data.UnitOfWork;
using DND.Common.Infrastructure.Validation;
using DND.Data;
using DND.Domain.Blog.BlogPosts;
using DND.Interfaces.Blog.DomainServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DND.DomainServices.Blog.BlogPosts.Services
{
    public class BlogPostDomainService : DomainServiceEntityBase<ApplicationContext, BlogPost>, IBlogPostDomainService
    {
        public BlogPostDomainService(IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory)
        : base(baseUnitOfWorkScopeFactory)
        {

        }

        public override void AddIncludes(List<Expression<Func<BlogPost, object>>> includes)
        {
            includes.Add(p => p.Tags);
            includes.Add(p => p.Locations);
        }

        public async Task<int> GetTotalPostsAsync(bool checkIsPublished, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetCountAsync(cancellationToken, p => !checkIsPublished || p.Published).ConfigureAwait(false);
            }

        }

        public IEnumerable<BlogPost> GetPosts(int pageNo, int pageSize)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().Get(p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsyncWithLocation(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForCarouselAsync(int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published && p.ShowInCarousel, o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForAuthorAsync(string authorSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published && p.Author.UrlSlug.Equals(authorSlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForAuthorAsync(string authorSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetCountAsync(cancellationToken, p => p.Published && p.Author.UrlSlug.Equals(authorSlug)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForCategoryAsync(string categorySlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published && p.Category.UrlSlug.Equals(categorySlug), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForCategoryAsync(string categorySlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetCountAsync(cancellationToken, p => p.Published && p.Category.UrlSlug.Equals(categorySlug)).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForTagAsync(string tagSlug, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug)), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForTagAsync(string tagSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetCountAsync(cancellationToken, p => p.Published && p.Tags.Any(t => t.Tag.UrlSlug.Equals(tagSlug))).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<BlogPost>> GetPostsForSearchAsync(string search, int pageNo, int pageSize, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetAsync(cancellationToken, p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search)) || p.Locations.Any(l => l.Location.Name.Equals(search))), o => o.OrderByDescending(p => p.DateCreated), pageNo * pageSize, pageSize, false, true).ConfigureAwait(false);
            }
        }

        public async Task<int> GetTotalPostsForSearchAsync(string search, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetCountAsync(cancellationToken, p => p.Published && (p.Title.Contains(search) || p.Category.Name.Equals(search) || p.Author.Name.Equals(search) || p.Tags.Any(t => t.Tag.Name.Equals(search))) || p.Locations.Any(l => l.Location.Name.Equals(search))).ConfigureAwait(false);
            }
        }

        public async Task<BlogPost> GetPostAsync(int year, int month, string titleSlug, CancellationToken cancellationToken)
        {
            using (var UoW = UnitOfWorkFactory.CreateReadOnly(UnitOfWorkScopeOption.JoinExisting))
            {
                return await UoW.ReadOnlyRepository<ApplicationContext, BlogPost>().GetFirstAsync(cancellationToken, p => p.DateCreated.Year == year && p.DateCreated.Month == month && p.UrlSlug.Equals(titleSlug), null, false, true).ConfigureAwait(false);
            }
        }

        #region "Admin"
        public async override Task<Result<BlogPost>> CreateAsync(CancellationToken cancellationToken, BlogPost entity, string createdBy)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Title);
            }

            return await base.CreateAsync(cancellationToken, entity, createdBy).ConfigureAwait(false);
        }

        public async Task<Result> UpdateAsync(BlogPost entity, IEnumerable<BlogPostTag> insertTags, IEnumerable<BlogPostTag> deleteTags, IEnumerable<BlogPostLocation> insertLocations, IEnumerable<BlogPostLocation> deleteLocations, string updatedBy, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(entity.UrlSlug))
            {
                entity.UrlSlug = UrlSlugger.ToUrlSlug(entity.Title);
            }

            using (var UoW = UnitOfWorkFactory.Create(UnitOfWorkScopeOption.JoinExisting))
            {

                UoW.Repository<ApplicationContext, BlogPost>().UpdateGraph(entity);

                await UoW.CompleteAsync(cancellationToken).ConfigureAwait(false);
            }

            return Result.Ok();
        }
        #endregion
    }
}