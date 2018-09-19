using DND.Common.SignalRHubs;
using DND.Web.FlightSearch.MVCImplementation.FlightSearch.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.FlightSearch.MVCImplementation.FlightSearch.Notifications
{
    public class FlightSearchNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<FlightSearchHub>(signalRUrlPrefix + "/signalr/flight-search");
        }
    }
}
