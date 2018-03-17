using Solution.Base.Interfaces.DomainServices;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface IBlogDomainService : IBaseDomainService
    {
        IBlogPostDomainService BlogPostDomainService { get; }
        ICategoryDomainService CategoryDomainService { get; }
        ITagDomainService TagDomainService { get; }
        IAuthorDomainService AuthorDomainService { get; }

    }
}
