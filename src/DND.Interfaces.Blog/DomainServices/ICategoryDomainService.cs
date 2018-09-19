using DND.Common.Infrastructure.Interfaces.DomainServices;
using DND.Domain.Blog.Categories;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.Blog.DomainServices
{
    public interface ICategoryDomainService : IDomainServiceEntity<Category>
    {
        Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
