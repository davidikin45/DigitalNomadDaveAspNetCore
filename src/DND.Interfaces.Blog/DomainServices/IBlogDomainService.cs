using DND.Common.Infrastructure.Interfaces.DomainServices;

namespace DND.Interfaces.Blog.DomainServices
{
    public interface IBlogDomainService : IDomainService
    {
        IBlogPostDomainService BlogPostDomainService { get; }
        ICategoryDomainService CategoryDomainService { get; }
        ITagDomainService TagDomainService { get; }
        IAuthorDomainService AuthorDomainService { get; }

    }
}
