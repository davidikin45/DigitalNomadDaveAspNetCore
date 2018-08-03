using DND.Common.SignalRHubs;
using DND.Web.MVCImplementation.FlightSearch.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web
{
    public static class SignalRConfig
    {
        public static void MapHubs(HubRouteBuilder routes, ISignalRHubMapper signalRHubMapper, string signalRUrlPrefix)
        {
            routes.MapHub<NotificationHub>("/api/signalr/notifications");
            routes.MapHub<FlightSearchHub>("/api/signalr/flight-search");

            signalRHubMapper.MapHubs(routes, signalRUrlPrefix);
        }
    }
}
