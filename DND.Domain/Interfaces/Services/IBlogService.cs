using Solution.Base.Interfaces.Services;

namespace DND.Domain.Interfaces.Services
{
    public interface IBlogService : IBaseBusinessService
    {
        IBlogPostService BlogPostService { get; }
        ICategoryService CategoryService { get; }
        ITagService TagService { get; }
        IAuthorService AuthorService { get; }

    }
}
