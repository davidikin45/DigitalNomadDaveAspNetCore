﻿using DND.Common.Infrastructure.Interfaces.ApplicationServices;
using DND.Domain.FlightSearch.Locales.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.ApplicationServices
{
    public interface ILocaleApplicationService : IApplicationService
    {
        Task<IEnumerable<LocaleDto>> GetAllAsync(CancellationToken cancellationToken);
        Task<LocaleDto> GetAsync(string id, CancellationToken cancellationToken);
    }
}
