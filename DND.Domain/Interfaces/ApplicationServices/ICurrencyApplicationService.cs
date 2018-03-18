using DND.Domain.DTOs;
using DND.Common.Interfaces.ApplicationServices;
using DND.Common.Interfaces.Services;
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
