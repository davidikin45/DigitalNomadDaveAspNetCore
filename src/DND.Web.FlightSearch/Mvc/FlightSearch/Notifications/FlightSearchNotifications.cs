using DND.Common.SignalRHubs;
using DND.Web.FlightSearch.Mvc.FlightSearch.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.FlightSearch.Mvc.FlightSearch.Notifications
{
    public class FlightSearchNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<FlightSearchHub>(signalRUrlPrefix + "/signalr/flight-search");
        }
    }
}
