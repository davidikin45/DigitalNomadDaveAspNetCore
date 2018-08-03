using DND.Common.SignalRHubs;
using DND.Domain.Blog.Authors.Dtos;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DND.Web.MVCImplementation.Author.Notifications
{
    public class AuthorsNotifications : ISignalRHubMap
    {
        public void MapHub(HubRouteBuilder routes, string signalRUrlPrefix)
        {
            routes.MapHub<ApiNotificationHub<AuthorDto>>(signalRUrlPrefix + "/authors/notifications");
        }
    }
}
