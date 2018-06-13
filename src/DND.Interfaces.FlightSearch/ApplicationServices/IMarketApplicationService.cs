﻿using DND.Common.Interfaces.ApplicationServices;
using DND.Domain.FlightSearch.Markets.Dtos;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Interfaces.FlightSearch.ApplicationServices
{
    public interface IMarketApplicationService : IBaseApplicationService
    {
        Task<IEnumerable<MarketDto>> GetByLocale(string locale, CancellationToken cancellationToken);
        Task<MarketDto> GetAsync(string id, CancellationToken cancellationToken);
    }
}