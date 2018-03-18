using DND.Common.Interfaces.ApplicationServices;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface IBlogApplicationService : IBaseApplicationService
    {
        IBlogPostApplicationService BlogPostApplicationService { get; }
        ICategoryApplicationService CategoryApplicationService { get; }
        ITagApplicationService TagApplicationService { get; }
        IAuthorApplicationService AuthorApplicationService { get; }

    }
}
