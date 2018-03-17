using DND.Domain.DTOs;
using Solution.Base.Interfaces.ApplicationServices;
using Solution.Base.Interfaces.Services;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.ApplicationServices
{
    public interface ILocaleApplicationService : IBaseApplicationService
    {
        Task<IEnumerable<LocaleDTO>> GetAllAsync(CancellationToken cancellationToken);
        Task<LocaleDTO> GetAsync(string id, CancellationToken cancellationToken);
    }
}
