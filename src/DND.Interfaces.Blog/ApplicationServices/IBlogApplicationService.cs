using DND.Common.Interfaces.ApplicationServices;

namespace DND.Interfaces.Blog.ApplicationServices
{
    public interface IBlogApplicationService : IBaseApplicationService
    {
        IBlogPostApplicationService BlogPostApplicationService { get; }
        ICategoryApplicationService CategoryApplicationService { get; }
        ITagApplicationService TagApplicationService { get; }
        IAuthorApplicationService AuthorApplicationService { get; }

    }
}
