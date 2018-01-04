using DND.Services.FlightSearch.BusinessObjects;
using DND.Services.Skyscanner.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DND.Services.SearchEngines.Interfaces
{
    public interface ILocationSearchEngine
    {
        Task<LocationServiceResponse> SearchByQueryAsync(string country, string currency, string locale, string query, CancellationToken cancellationToken);

        Task<Place> GetByIDAsync(string country, string currency, string locale, string id, CancellationToken cancellationToken);

    }
}
