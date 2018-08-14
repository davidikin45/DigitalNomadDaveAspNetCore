﻿using DND.Common.SignalRHubs;
using DND.Domain.Blog.Tags.Dtos;
using Microsoft.AspNetCore.SignalR;

namespace DND.Web.Blog.MVCImplementation.Tags.Notifications
{
    public class FormsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<TagDto>>(signalRUrlPrefix + "/blog/tags/notifications");
        }
    }
}