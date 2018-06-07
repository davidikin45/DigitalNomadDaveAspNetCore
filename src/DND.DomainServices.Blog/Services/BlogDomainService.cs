using DND.Common.Implementation.DomainServices;
using DND.Common.Interfaces.UnitOfWork;
using DND.Interfaces.Blog.DomainServices;

namespace DND.DomainServices.Blog.Services
{
    public class BlogDomainService : BaseDomainService, IBlogDomainService
    {
        public IBlogPostDomainService BlogPostDomainService { get; private set; }
        public ICategoryDomainService CategoryDomainService { get; private set; }
        public ITagDomainService TagDomainService { get; private set; }
        public IAuthorDomainService AuthorDomainService { get; private set; }

        public BlogDomainService(
            IUnitOfWorkScopeFactory baseUnitOfWorkScopeFactory,
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
}