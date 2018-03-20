using DND.Common.Interfaces.DomainServices;
using DND.Domain.Blog.Categories;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface ICategoryDomainService : IBaseEntityDomainService<Category>
    {
        Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
