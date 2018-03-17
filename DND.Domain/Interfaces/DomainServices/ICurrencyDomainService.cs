using DND.Domain.DTOs;
using DND.Domain.Skyscanner.Model;
using Solution.Base.Interfaces.DomainServices;
using Solution.Base.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface ILocaleDomainService : IBaseDomainService
    {
        Task<IEnumerable<Locale>> GetAllAsync(CancellationToken cancellationToken);
        Task<Locale> GetAsync(string id, CancellationToken cancellationToken);
    }
}
