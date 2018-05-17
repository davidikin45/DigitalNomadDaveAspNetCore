﻿using DND.Common.Interfaces.DomainServices;
using DND.Domain.Skyscanner.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Domain.Interfaces.DomainServices
{
    public interface IMarketDomainService : IBaseDomainService
    {
        Task<IEnumerable<Country>> GetByLocale(string locale, CancellationToken cancellationToken);
        Task<Country> GetAsync(string id, CancellationToken cancellationToken);
    }
}