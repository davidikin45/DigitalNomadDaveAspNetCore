using DND.Domain.DTOs;
using DND.Domain.Models;
using DND.Common.Interfaces.DomainServices;
using DND.Common.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface ICategoryDomainService : IBaseEntityDomainService<Category>
    {
        Task<Category> GetCategoryAsync(string categorySlug, CancellationToken cancellationToken);
    }
}
