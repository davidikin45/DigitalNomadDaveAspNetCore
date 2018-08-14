﻿using DND.Common.SignalRHubs;
using DND.Domain.Blog.Locations.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Blog.MVCImplementation.Locations.Notifications
{
    public class LocationsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<LocationDto>>(signalRUrlPrefix + "/blog/locations/notifications");
        }
    }
}