using DND.Common.Infrastructure.Interfaces.ApplicationServices;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface IBlogApplicationService : IApplicationService
    {
        IBlogPostApplicationService BlogPostApplicationService { get; }
        ICategoryApplicationService CategoryApplicationService { get; }
        ITagApplicationService TagApplicationService { get; }
        IAuthorApplicationService AuthorApplicationService { get; }

    }
}
