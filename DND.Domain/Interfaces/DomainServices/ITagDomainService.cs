using DND.Domain.DTOs;
using DND.Domain.Models;
using Solution.Base.Interfaces.DomainServices;
using Solution.Base.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface ITagDomainService : IBaseEntityDomainService<Tag>
    {
        Task<Tag> GetTagAsync(string tagSlug, CancellationToken cancellationToken);
    }
}
