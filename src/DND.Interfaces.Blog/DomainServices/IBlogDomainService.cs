using DND.Common.Interfaces.DomainServices;

namespace DND.Interfaces.Blog.DomainServices
{
    public interface IBlogDomainService : IBaseDomainService
    {
        IBlogPostDomainService BlogPostDomainService { get; }
        ICategoryDomainService CategoryDomainService { get; }
        ITagDomainService TagDomainService { get; }
        IAuthorDomainService AuthorDomainService { get; }

    }
}
